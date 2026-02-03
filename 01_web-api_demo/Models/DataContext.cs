using Microsoft.EntityFrameworkCore;

namespace _01_web_api_demo.Models;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }
    public DbSet<Product> Products { get; set; }
}
