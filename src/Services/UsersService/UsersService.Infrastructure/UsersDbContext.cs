using Microsoft.EntityFrameworkCore;
using UsersService.Domain.Entities;

namespace UsersService.Infrastructure;

public class UsersDbContext : DbContext
{
    public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // tabla y columnas con los nombres exactos usados en SQL/migraciones
        modelBuilder.Entity<User>(b =>
        {
            b.ToTable("users");
            b.HasKey(x => x.Id).HasName("pk_users");

            b.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid")
             .HasDefaultValueSql("gen_random_uuid()");

            b.Property(x => x.Email).HasColumnName("email").HasColumnType("text").IsRequired();
            b.HasIndex(x => x.Email).IsUnique().HasDatabaseName("ux_users_email");

            b.Property(x => x.Name).HasColumnName("name").HasColumnType("text").IsRequired();

            b.Property(x => x.PasswordHash).HasColumnName("password_hash").HasColumnType("text").IsRequired();
            b.Property(x => x.PasswordSalt).HasColumnName("password_salt").HasColumnType("text").IsRequired();

            b.Property(x => x.Role)
              .HasConversion<string>()      // ← guarda “Admin” / “Usuario” como TEXT
              .HasColumnName("role")
              .HasColumnType("text")
              .IsRequired();

            b.Property(x => x.IsActive).HasColumnName("is_active").HasColumnType("boolean").HasDefaultValue(true);
            b.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz").HasDefaultValueSql("now()");
            b.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamptz");
        });
    }
}
