using Microsoft.EntityFrameworkCore;

namespace _05_authentication.Models;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions options) : base(options) { }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Account>()
            .HasIndex(acc => acc.Email)
            .IsUnique();
        modelBuilder.Entity<RefreshTokenRecord>()
            .HasIndex(rft => rft.Id)
            .IsUnique();

        modelBuilder.Entity<RefreshTokenRecord>()
            .HasOne(rft => rft.Account)
            .WithMany(rft => rft.RefreshTokens)
            .HasForeignKey(rft => rft.AccountId);
    }

    public DbSet<Account> Accounts { get; set; }
    public DbSet<RefreshTokenRecord> RefreshTokenRecords { get; set; }
}