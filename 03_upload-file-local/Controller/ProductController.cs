using _03_upload_file_local.Data;
using LModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _03_upload_file_local.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        public DataContext _dbContext;
        public ProductController(DataContext context) => _dbContext = context;

        [HttpGet]
        public async Task<ActionResult> GetProducts()
        {
            var products = await _dbContext.Products.ToListAsync();
            return Ok(products);
        }

        [HttpPost]
        public async Task<ActionResult> AddProduct(Product product)
        {
            await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveChangesAsync();
            return Ok(product);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetProduct(int id)
        {
            var product = await _dbContext.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound("Product not found");
            }
            return Ok(product);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateProduct(int id, Product pro)
        {
            // var proExisting = await _dbContext.Products.FindAsync(id);
            var proExisting = await _dbContext.Products
                .AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            if (proExisting is null) return NotFound();

            pro.Id = id;
            _dbContext.Products.Update(pro);

            await _dbContext.SaveChangesAsync();
            return Ok(pro);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var product = await _dbContext.Products.FindAsync(id);
            if (product is null) return NotFound("Product not found");
            _dbContext.Products.Remove(product);
            await _dbContext.SaveChangesAsync();
            return Ok("Product deleted successfully");
        }
    }
}