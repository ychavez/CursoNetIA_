using AulaPedidos.Application.Products;
using AulaPedidos.Application.Validation;
using FluentValidation;
using Mediator;
using Xunit;

namespace AulaPedidos.Api.IntegrationTests;

public sealed class ValidationBehaviorTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ValidOrUnvalidatedMessageContinuesExactlyOnce(bool hasValidator)
    {
        var message = new GetProductsQuery();
        using var cancellation = new CancellationTokenSource();
        var calls = 0;
        var validatorCalls = new int[2];
        var first = new InlineValidator<GetProductsQuery>();
        first.RuleFor(query => query).MustAsync(async (query, token) =>
        {
            await Task.Yield();
            validatorCalls[0]++;
            Assert.Same(message, query);
            Assert.Equal(cancellation.Token, token);
            return true;
        });
        var second = new InlineValidator<GetProductsQuery>();
        second.RuleFor(query => query).MustAsync((query, token) =>
        {
            validatorCalls[1]++;
            Assert.Same(message, query);
            Assert.Equal(cancellation.Token, token);
            return Task.FromResult(true);
        });
        var behavior = new ValidationBehavior<GetProductsQuery, object>(hasValidator ? [first, second] : []);
        var expected = new object();

        var result = await behavior.Handle(message, (query, token) =>
        {
            calls++;
            Assert.Same(message, query);
            Assert.Equal(cancellation.Token, token);
            return ValueTask.FromResult(expected);
        }, cancellation.Token);

        Assert.Same(expected, result);
        Assert.Equal(1, calls);
        int[] expectedCalls = hasValidator ? [1, 1] : [0, 0];
        Assert.Equal(expectedCalls, validatorCalls);
    }

    [Fact]
    public async Task ValidCommandRunsEachValidatorAndHandlerExactlyOnce()
    {
        var message = new CreateProductCommand(1, null, null);
        using var cancellation = new CancellationTokenSource();
        var validatorCalls = new int[2];
        var validators = Enumerable.Range(0, 2).Select(index =>
        {
            var validator = new InlineValidator<CreateProductCommand>();
            validator.RuleFor(command => command).MustAsync((command, token) =>
            {
                validatorCalls[index]++;
                Assert.Same(message, command);
                Assert.Equal(cancellation.Token, token);
                return Task.FromResult(true);
            });
            return validator;
        }).ToArray();
        var behavior = new ValidationBehavior<CreateProductCommand, int>(validators);
        var handlerCalls = 0;

        var result = await behavior.Handle(message, (command, token) =>
        {
            handlerCalls++;
            Assert.Same(message, command);
            Assert.Equal(cancellation.Token, token);
            return ValueTask.FromResult(42);
        }, cancellation.Token);

        Assert.Equal(42, result);
        Assert.Equal([1, 1], validatorCalls);
        Assert.Equal(1, handlerCalls);
    }

    [Fact]
    public async Task CollectsAllFailuresBeforeBlockingHandler()
    {
        var first = new InlineValidator<CreateProductCommand>();
        first.RuleFor(command => command.Price).Custom((_, context) => context.AddFailure("E1"));
        first.RuleFor(command => command.Description).Custom((_, context) => context.AddFailure("E2"));
        var second = new InlineValidator<CreateProductCommand>();
        second.RuleFor(command => command.Price).Custom((_, context) => context.AddFailure("E3"));
        var lastCalled = false;
        var last = new InlineValidator<CreateProductCommand>();
        last.RuleFor(command => command).Custom((_, _) => lastCalled = true);
        var calls = 0;
        var behavior = new ValidationBehavior<CreateProductCommand, int>([first, second, last]);

        var exception = await Assert.ThrowsAsync<ValidationException>(() => behavior.Handle(
            new(1, null, null), (_, _) => { calls++; return ValueTask.FromResult(1); }, default).AsTask());

        Assert.Equal(["Price:E1", "Description:E2", "Price:E3"],
            exception.Errors.Select(failure => $"{failure.PropertyName}:{failure.ErrorMessage}"));
        Assert.True(lastCalled);
        Assert.Equal(0, calls);
    }

    [Fact]
    public async Task ValidatorsRunSequentiallyWithTheOriginalToken()
    {
        using var cancellation = new CancellationTokenSource();
        var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var secondStarted = false;
        var first = new InlineValidator<GetProductsQuery>();
        first.RuleFor(query => query).MustAsync(async (_, token) =>
        {
            Assert.Equal(cancellation.Token, token);
            started.SetResult();
            await release.Task.WaitAsync(token);
            return true;
        });
        var second = new InlineValidator<GetProductsQuery>();
        second.RuleFor(query => query).MustAsync((_, token) =>
        {
            Assert.Equal(cancellation.Token, token);
            Assert.True(release.Task.IsCompleted);
            secondStarted = true;
            return Task.FromResult(true);
        });
        var behavior = new ValidationBehavior<GetProductsQuery, int>([first, second]);
        var pending = behavior.Handle(new(), (_, _) => ValueTask.FromResult(42), cancellation.Token).AsTask();

        await started.Task.WaitAsync(TimeSpan.FromSeconds(10));
        try
        {
            Assert.False(secondStarted);
            Assert.False(pending.IsCompleted);
        }
        finally
        {
            release.TrySetResult();
        }
        Assert.Equal(42, await pending);
        Assert.True(secondStarted);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task CancellationNeverContinuesOrBecomesValidation(bool alreadyCancelled)
    {
        using var cancellation = new CancellationTokenSource();
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var first = new InlineValidator<GetProductsQuery>();
        first.RuleFor(query => query).MustAsync(async (_, token) =>
        {
            entered.SetResult();
            await Task.Delay(Timeout.Infinite, token);
            return true;
        });
        var second = new InlineValidator<GetProductsQuery>();
        second.RuleFor(query => query).Custom((_, _) => Assert.Fail("No debe empezar otro validador."));
        var behavior = new ValidationBehavior<GetProductsQuery, int>([first, second]);
        if (alreadyCancelled)
        {
            cancellation.Cancel();
        }
        var pending = behavior.Handle(new(), (_, _) => throw new InvalidOperationException("Handler inesperado"),
            cancellation.Token).AsTask();
        if (!alreadyCancelled)
        {
            await entered.Task.WaitAsync(TimeSpan.FromSeconds(10));
            cancellation.Cancel();
        }

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => pending);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task UnexpectedFailuresKeepTheirIdentity(bool validatorFails)
    {
        var expected = new InvalidOperationException("TECHNICAL_SENTINEL");
        var validator = new InlineValidator<GetProductsQuery>();
        validator.RuleFor(query => query).Custom((_, _) =>
        {
            if (validatorFails) throw expected;
        });
        var calls = 0;
        var behavior = new ValidationBehavior<GetProductsQuery, int>([validator]);

        var actual = await Assert.ThrowsAsync<InvalidOperationException>(() => behavior.Handle(new(), (_, _) =>
        {
            calls++;
            throw expected;
        }, default).AsTask());

        Assert.Same(expected, actual);
        Assert.Equal(validatorFails ? 0 : 1, calls);
    }
}
