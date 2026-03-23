using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Model;

namespace server.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly DataContext _dbContext;
        public ProductController(DataContext dbContext)
            => _dbContext = dbContext;

        [HttpGet]
        public async Task<IActionResult> GetAllProducts(decimal? minPrice, decimal? maxPrice)
        {
            var query = _dbContext.Products.AsQueryable();

            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice.Value);

            var products = await query.ToListAsync();
            return Ok(products);
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] Product pro)
        {
            if (!ModelState.IsValid) return BadRequest();

            await _dbContext.Products.AddAsync(pro);
            await _dbContext.SaveChangesAsync();

            return Ok(pro);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateProductStatus(int id, [FromQuery] bool status)
        {
            var existingPro = await _dbContext.Products.FindAsync(id);
            if (existingPro is null) return NotFound();

            existingPro.Status = status;

            _dbContext.Products.Update(existingPro);
            await _dbContext.SaveChangesAsync();

            return Ok(existingPro);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var existingPro = await _dbContext.Products.FindAsync(id);
            if (existingPro is null) return NotFound();

            _dbContext.Remove(existingPro);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
