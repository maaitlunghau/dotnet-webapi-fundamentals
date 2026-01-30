using LModels;
using Microsoft.EntityFrameworkCore;

namespace _03_upload_file_local.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    public DbSet<Product> Products { get; set; }
}
