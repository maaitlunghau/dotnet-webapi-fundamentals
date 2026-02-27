using Microsoft.EntityFrameworkCore;
using Shared.Domain;

namespace backend.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<RefreshTokenRecord>()
            .HasOne(rft => rft.User)
            .WithMany(u => u.RefreshTokenRecords)
            .HasForeignKey(rft => rft.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "Mai Trung Hau",
                Email = "trunghau@mstsoftware.vn",
                Password = BCrypt.Net.BCrypt.HashPassword("admin@123"),
                Role = "admin",
                CreatedAtUTC = DateTime.UtcNow,
                UpdatedAtUTC = DateTime.UtcNow
            },
            new User
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "John Doe",
                Email = "john@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword("user123"),
                Role = "user",
                CreatedAtUTC = DateTime.UtcNow,
                UpdatedAtUTC = DateTime.UtcNow
            },
            new User
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = "Jane Smith",
                Email = "jane@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword("user123"),
                Role = "user",
                CreatedAtUTC = DateTime.UtcNow,
                UpdatedAtUTC = DateTime.UtcNow
            },
            new User
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Name = "Bob Johnson",
                Email = "bob@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword("user123"),
                Role = "user",
                CreatedAtUTC = DateTime.UtcNow,
                UpdatedAtUTC = DateTime.UtcNow
            },
            new User
            {
                Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                Name = "Alice Williams",
                Email = "alice@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword("user123"),
                Role = "user",
                CreatedAtUTC = DateTime.UtcNow,
                UpdatedAtUTC = DateTime.UtcNow
            }
        );
    }

    public DbSet<User> Users { get; set; }
    public DbSet<RefreshTokenRecord> RefreshTokenRecords { get; set; }
}
