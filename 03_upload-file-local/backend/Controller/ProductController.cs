using System.Net;
using backend.Data;
using LModels.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly DataContext _dbContext;
        public ProductController(DataContext dbContext) => _dbContext = dbContext;


        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var products = await _dbContext.Products.ToListAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                var pro = await _dbContext.Products.FindAsync(id);
                if (pro is null) return NotFound();

                return Ok(pro);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Product product)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await _dbContext.Products.AddAsync(product);
                await _dbContext.SaveChangesAsync();

                return CreatedAtAction(
                    nameof(GetProductById),
                    new { id = product.Id },
                    product
                );
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product pro)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var existingPro = await _dbContext.Products.FindAsync(id);
                if (existingPro is null) return NotFound();

                existingPro.Name = pro.Name;
                existingPro.Price = pro.Price;
                existingPro.ImageUrl = pro.ImageUrl;

                await _dbContext.SaveChangesAsync();
                return Ok(existingPro);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var pro = await _dbContext.Products.FindAsync(id);
                if (pro is null) return NotFound();

                _dbContext.Products.Remove(pro);
                await _dbContext.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}