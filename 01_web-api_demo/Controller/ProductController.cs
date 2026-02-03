using _01_web_api_demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

// Workflow:
// First do simply, after do complex (ApiResponse class, AsNoTracking method)

namespace _01_web_api_demo.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly DataContext _dbContext;
        public ProductController(DataContext dbContext) => _dbContext = dbContext;

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var products = await _dbContext.Products.ToListAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetOneProduct(Guid id)
        {
            try
            {
                var pro = await _dbContext.Products.FindAsync(id);
                if (pro is null) return NotFound();

                return Ok(pro);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] Product pro)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _dbContext.Products.AddAsync(pro);
                await _dbContext.SaveChangesAsync();

                return Created("Created new successfully", pro); // HTTP 201 & message
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // Cách 1: Update API với method
        // [HttpPut("{id:Guid}")]
        // public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] Product pro)
        // {
        //     if (!ModelState.IsValid)
        //     {
        //         return BadRequest(ModelState);
        //     }

        //     try
        //     {
        //         var existingProduct = await _dbContext.Products.FindAsync(id);
        //         if (existingProduct is null) return NotFound();

        //         pro.Id = id;

        //         _dbContext.Products.Update(pro);
        //         await _dbContext.SaveChangesAsync();

        //         return Ok(pro);
        //     }
        //     catch (Exception ex)
        //     {
        //         return StatusCode(500, ex.Message);
        //     }
        // }

        // Cách 2: Update API với từng property -> tốt hơn và đúng với thực tế hơn
        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] Product pro)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var existingProduct = await _dbContext.Products.FindAsync(id);
                if (existingProduct is null) return NotFound();

                existingProduct.Name = pro.Name;
                existingProduct.Price = pro.Price;
                existingProduct.Stock = pro.Stock;
                existingProduct.IsActive = pro.IsActive;
                existingProduct.Description = pro.Description;

                await _dbContext.SaveChangesAsync();
                return Ok(existingProduct); // JSON data + HTTP 200
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            try
            {
                var existingProduct = await _dbContext.Products.FindAsync(id);
                if (existingProduct is null) return NotFound();

                _dbContext.Products.Remove(existingProduct);
                await _dbContext.SaveChangesAsync();

                return NoContent(); // 204 - no body (no message / data)
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
