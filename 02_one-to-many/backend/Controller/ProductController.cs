using System.Net;
using backend.Data;
using backend.DTOs;
using LModels.Domain;
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
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var products = await _dbContext.Products
                    .Include(p => p.Category)
                    .Select(p => new ProductDto(
                        p.Id,
                        p.Name,
                        p.Price,
                        p.Stock,
                        p.CategoryId,
                        p.Category != null ? p.Category.Name : "Unknown"
                    ))
                    .ToListAsync();

                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                var existingPro = await _dbContext.Products
                    .Include(p => p.Category)
                    .Where(p => p.Id == id)
                    .Select(p => new ProductDto(
                        p.Id,
                        p.Name,
                        p.Price,
                        p.Stock,
                        p.CategoryId,
                        p.Category != null ? p.Category.Name : "Unknown"
                    ))
                    .FirstOrDefaultAsync();
                if (existingPro is null)
                    return NotFound($"Product with Id = {id} not found.");

                return Ok(existingPro);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("category/{categoryId:int}")]
        public async Task<IActionResult> GetProductsByCategory(int categoryId)
        {
            try
            {
                var categoryExisting = await _dbContext.Categories.FindAsync(categoryId);
                if (categoryExisting is null)
                    return NotFound($"Category with Id = {categoryId} not found.");

                var products = await _dbContext.Products
                    .Include(p => p.Category)
                    .Where(p => p.CategoryId == categoryId)
                    .Select(p => new ProductDto(
                        p.Id,
                        p.Name,
                        p.Price,
                        p.Stock,
                        p.CategoryId,
                        p.Category != null ? p.Category.Name : "Unknown"
                    ))
                    .ToListAsync();

                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto pro)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var existingCategory = await _dbContext.Categories
                    .AnyAsync(c => c.Id == pro.CategoryId);
                if (!existingCategory)
                    return BadRequest($"Category with Id = {pro.CategoryId} not found.");

                var entity = new Product
                {
                    Name = pro.Name,
                    Price = pro.Price,
                    Stock = pro.Stock,
                    CategoryId = pro.CategoryId
                };

                await _dbContext.Products.AddAsync(entity);
                await _dbContext.SaveChangesAsync();

                var category = await _dbContext.Categories.FindAsync(entity.CategoryId);
                var result = new ProductDto(
                    entity.Id,
                    entity.Name,
                    entity.Price,
                    entity.Stock,
                    entity.CategoryId,
                    category != null ? category.Name : "Unknown"
                );

                return CreatedAtAction(
                    nameof(GetProductById),
                    new { id = entity.Id },
                    result
                );
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDto pro)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var existingCategory = await _dbContext.Categories
                    .AnyAsync(c => c.Id == pro.CategoryId);
                if (!existingCategory)
                    return BadRequest($"Category with Id = {pro.CategoryId} not found.");

                var existingProduct = await _dbContext.Products.FindAsync(id);
                if (existingProduct is null)
                    return BadRequest($"Product with Id = {id} not found.");

                existingProduct.Name = pro.Name;
                existingProduct.Price = pro.Price;
                existingProduct.Stock = pro.Stock;
                existingProduct.CategoryId = pro.CategoryId;

                await _dbContext.SaveChangesAsync();

                var category = await _dbContext.Categories.FindAsync(existingProduct.CategoryId);
                var result = new ProductDto(
                    existingProduct.Id,
                    existingProduct.Name,
                    existingProduct.Price,
                    existingProduct.Stock,
                    existingProduct.CategoryId,
                    category != null ? category.Name : "Unknown"
                );

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var existingProduct = await _dbContext.Products.FindAsync(id);
                if (existingProduct is null)
                    return NotFound($"Product with Id = {id} not found.");

                _dbContext.Products.Remove(existingProduct);
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