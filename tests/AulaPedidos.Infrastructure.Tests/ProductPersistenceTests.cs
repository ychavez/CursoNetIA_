using AulaPedidos.Domain;
using AulaPedidos.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Xunit;

namespace AulaPedidos.Infrastructure.Tests;

public sealed class ProductPersistenceTests
{
    [Fact]
    public async Task MigrationIsRepeatableAndPreservesUnrelatedDataInBothDirections()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(connection).Options;
        await using (var context = new AppDbContext(options))
        {
            await context.Database.ExecuteSqlRawAsync("CREATE TABLE ExistingData (Id INTEGER PRIMARY KEY, Value TEXT NOT NULL)");
            await context.Database.ExecuteSqlRawAsync("INSERT INTO ExistingData VALUES (1, 'preserved')");
            Assert.False(context.Database.HasPendingModelChanges());
            await context.Database.MigrateAsync();
            await context.Database.MigrateAsync();
            Assert.Single(await context.Database.GetAppliedMigrationsAsync());
            var repository = new Repository<Product>(context);
            await repository.AddAsync(new Product(Guid.NewGuid(), 12.345m, "persisted", null));
            await new UnitOfWork(context).SaveChangesAsync();
        }

        await using (var context = new AppDbContext(options))
        {
            var repository = new Repository<Product>(context);
            var product = Assert.Single(await repository.GetAsync());
            Assert.Empty(context.ChangeTracker.Entries());
            Assert.Equal(12.345m, product.Price);
            Assert.Equal("persisted", product.Description);
            Assert.Equal("preserved", await context.Database.SqlQueryRaw<string>(
                "SELECT Value FROM ExistingData").SingleAsync());

            // Sólo esta base efímera en memoria: comprueba también el rollback definido.
            await context.GetService<IMigrator>().MigrateAsync("0");
            Assert.Empty(await context.Database.GetAppliedMigrationsAsync());
            Assert.Equal("preserved", await context.Database.SqlQueryRaw<string>(
                "SELECT Value FROM ExistingData").SingleAsync());
        }
    }
}
