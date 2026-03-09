using Microsoft.EntityFrameworkCore;
using user_service.Model;

namespace user_service.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> option) : base(option) { }
    public DbSet<User> Users { get; set; }
}
