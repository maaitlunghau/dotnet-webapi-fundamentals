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
            .HasIndex(u => u.Id)
            .IsUnique();
        modelBuilder.Entity<RefreshTokenRecord>()
            .HasIndex(rft => rft.Id)
            .IsUnique();

        modelBuilder.Entity<RefreshTokenRecord>()
            .HasOne(u => u.User)
            .WithMany(rft => rft.RefreshTokenRecords)
            .HasForeignKey(rft => rft.UserId);
    }

    public DbSet<User> Users { get; set; }
    public DbSet<RefreshTokenRecord> RefreshTokenRecords { get; set; }
}