using Microsoft.EntityFrameworkCore;
using order_service.Models;

namespace order_service.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> option) : base(option) { }
    public DbSet<Order> Users { get; set; }
}
