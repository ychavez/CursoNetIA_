using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace AulaPedidos.Infrastructure.Persistence.Migrations;

[DbContext(typeof(AppDbContext))]
public sealed class AppDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("ProductVersion", "10.0.12");
        modelBuilder.Entity("AulaPedidos.Domain.Product", product =>
        {
            product.Property<Guid>("Id").HasColumnType("TEXT");
            product.Property<decimal>("Price").HasColumnType("TEXT");
            product.Property<string>("Description").HasColumnType("TEXT");
            product.Property<string>("AlternateDescription").HasColumnType("TEXT");
            product.HasKey("Id");
            product.ToTable("Products");
        });
    }
}
