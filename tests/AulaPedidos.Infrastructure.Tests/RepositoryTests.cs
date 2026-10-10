using AulaPedidos.Application.Abstractions.Persistence;
using AulaPedidos.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AulaPedidos.Infrastructure.Tests;

public sealed class RepositoryTests : IAsyncLifetime
{
    private readonly SqliteConnection _connection = new("Data Source=:memory:");
    private TestDbContext _context = null!;
    private Repository<TestEntity> _repository = null!;
    private UnitOfWork _unitOfWork = null!;

    public async Task InitializeAsync()
    {
        await _connection.OpenAsync();
        _context = CreateContext();
        await _context.Database.EnsureCreatedAsync();
        _repository = new Repository<TestEntity>(_context);
        _unitOfWork = new UnitOfWork(_context);
        _context.Set<TestEntity>().AddRange(
            new TestEntity { Name = "uno", Value = 1 },
            new TestEntity { Name = "dos", Value = 2 },
            new TestEntity { Name = "tres", Value = 3 },
            new TestEntity { Name = "cuatro", Value = 4 },
            new TestEntity { Name = "cinco", Value = 5 });
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
    }

    public async Task DisposeAsync()
    {
        await _context.DisposeAsync();
        await _connection.DisposeAsync();
    }

    private TestDbContext CreateContext() => new(
        new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_connection).Options);

    [Fact]
    public async Task GetAsyncFiltersWithThePredicateAndReturnsEverythingWithoutIt()
    {
        Assert.Equal(5, (await _repository.GetAsync()).Count);
        var filtered = await _repository.GetAsync(entity => entity.Value > 3);
        Assert.Equal(new[] { 4, 5 }, filtered.Select(entity => entity.Value).Order());
        Assert.Empty(await _repository.GetAsync(entity => entity.Value > 100));
    }

    [Fact]
    public async Task FirstOrDefaultAnyAndCountRespectThePredicate()
    {
        var found = await _repository.FirstOrDefaultAsync(entity => entity.Name == "dos");
        Assert.Equal(2, found?.Value);
        Assert.Null(await _repository.FirstOrDefaultAsync(entity => entity.Name == "cero"));
        Assert.True(await _repository.AnyAsync(entity => entity.Value == 1));
        Assert.False(await _repository.AnyAsync(entity => entity.Value == 99));
        Assert.Equal(5, await _repository.CountAsync());
        Assert.Equal(2, await _repository.CountAsync(entity => entity.Value <= 2));
    }

    [Fact]
    public async Task QueryComposesWithoutTrackingAndDefersExecution()
    {
        var query = _repository.Query(entity => entity.Value >= 2)
            .OrderByDescending(entity => entity.Value)
            .Select(entity => entity.Name);
        Assert.Empty(_context.ChangeTracker.Entries());
        var names = await query.ToListAsync();
        Assert.Equal(new[] { "cinco", "cuatro", "tres", "dos" }, names);
        Assert.Empty(_context.ChangeTracker.Entries());
    }

    [Fact]
    public async Task GetPagedAsyncReturnsIntermediateLastAndOutOfRangePages()
    {
        static IOrderedQueryable<TestEntity> OrderByValue(IQueryable<TestEntity> query) =>
            query.OrderBy(entity => entity.Value);

        var second = await _repository.GetPagedAsync(2, 2, null, OrderByValue, CancellationToken.None);
        Assert.Equal(new[] { 3, 4 }, second.Select(entity => entity.Value));
        var last = await _repository.GetPagedAsync(3, 2, null, OrderByValue, CancellationToken.None);
        Assert.Equal(new[] { 5 }, last.Select(entity => entity.Value));
        Assert.Empty(await _repository.GetPagedAsync(9, 2, null, OrderByValue, CancellationToken.None));
    }

    [Fact]
    public async Task GetPagedAsyncCombinesPredicateOrderAndIncludes()
    {
        var page = await _repository.GetPagedAsync(
            1,
            2,
            entity => entity.Value >= 2,
            query => query.OrderByDescending(entity => entity.Value),
            CancellationToken.None,
            nameof(TestEntity.Child));
        Assert.Equal(new[] { 5, 4 }, page.Select(entity => entity.Value));
    }

    [Theory]
    [InlineData(0, 2)]
    [InlineData(1, 0)]
    [InlineData(-3, -1)]
    public async Task GetPagedAsyncRejectsValuesBelowOne(int pageNumber, int pageSize)
    {
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            _repository.GetPagedAsync(pageNumber, pageSize, null, null, CancellationToken.None));
    }

    [Fact]
    public async Task WritesOnlyPersistAfterSaveChanges()
    {
        var entity = new TestEntity { Name = "seis", Value = 6 };
        await _repository.AddAsync(entity, CancellationToken.None);
        await using (var probe = CreateContext())
        {
            Assert.Equal(5, await probe.Set<TestEntity>().CountAsync());
        }

        Assert.Equal(1, await _unitOfWork.SaveChangesAsync(CancellationToken.None));
        await using (var probe = CreateContext())
        {
            Assert.Equal(6, await probe.Set<TestEntity>().CountAsync());
        }

        entity.Name = "seis-editado";
        _repository.Update(entity);
        Assert.Equal(1, await _unitOfWork.SaveChangesAsync(CancellationToken.None));

        _repository.Remove(entity);
        await using (var probe = CreateContext())
        {
            Assert.True(await probe.Set<TestEntity>().AnyAsync(stored => stored.Name == "seis-editado"));
        }

        Assert.Equal(1, await _unitOfWork.SaveChangesAsync(CancellationToken.None));
        await using (var final = CreateContext())
        {
            Assert.Equal(5, await final.Set<TestEntity>().CountAsync());
        }
    }

    [Fact]
    public async Task CancelledTokenStopsReadsAndWrites()
    {
        using var source = new CancellationTokenSource();
        await source.CancelAsync();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => _repository.GetAsync(null, source.Token));
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => _repository.CountAsync(null, source.Token));
        // AddAsync solo espera a los generadores de valores, por lo que aqui no observa la cancelacion.
        await _repository.AddAsync(new TestEntity { Name = "x" }, CancellationToken.None);
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => _unitOfWork.SaveChangesAsync(source.Token));
    }

    [Fact]
    public async Task NullEntitiesAreRejected()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _repository.AddAsync(null!, CancellationToken.None));
        Assert.Throws<ArgumentNullException>(() => _repository.Update(null!));
        Assert.Throws<ArgumentNullException>(() => _repository.Remove(null!));
        Assert.Throws<ArgumentNullException>(() => new Repository<TestEntity>(null!));
        Assert.Throws<ArgumentNullException>(() => new UnitOfWork(null!));
    }
}
