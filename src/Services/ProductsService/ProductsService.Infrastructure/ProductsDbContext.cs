using Microsoft.EntityFrameworkCore;
using ProductsService.Domain.Entities;

namespace ProductsService.Infrastructure;

public class ProductsDbContext : DbContext
{
    public ProductsDbContext(DbContextOptions<ProductsDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Si NO usas el mapeo nativo de enums de Npgsql, puedes mapear a texto con HasConversion<string>().
        // Aquí usaré tipo ENUM de Postgres y default literal SQL.
        modelBuilder.Entity<Product>(b =>
        {
            b.ToTable("products");
            b.HasKey(x => x.Id).HasName("pk_products");

            b.Property(x => x.Id)
                .HasColumnName("id")
                .HasColumnType("uuid")
                .HasDefaultValueSql("gen_random_uuid()");

            b.Property(x => x.Name)
                .HasColumnName("name")
                .HasColumnType("text")
                .IsRequired();

            b.Property(x => x.Description)
                .HasColumnName("description")
                .HasColumnType("text")
                .IsRequired();

            b.Property(x => x.Price)
                .HasColumnName("price")
                .HasColumnType("numeric(18,2)")
                .IsRequired();

            b.Property(x => x.Category)
                .HasColumnName("category")
                .HasColumnType("text")
                .IsRequired();

            b.Property(x => x.Status).HasColumnName("status").HasColumnType("boolean").HasDefaultValue(true);

            b.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamptz")
                .HasDefaultValueSql("now()");

            b.Property(x => x.UpdatedAt)
                .HasColumnName("updated_at")
                .HasColumnType("timestamptz");
        });
    }
}
