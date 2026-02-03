using backend.Data;
using backend.DTOs;
using LModels.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly DataContext _dbContext;
        public CategoryController(DataContext dbContext) => _dbContext = dbContext;

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var categories = await _dbContext.Categories
                    .Select(c => new CategoryDto(
                        c.Id,
                        c.Name
                    ))
                    .ToListAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            try
            {
                var category = await _dbContext.Categories
                    .Where(c => c.Id == id)
                    .Select(c => new CategoryDto(
                        c.Id,
                        c.Name
                    ))
                    .FirstOrDefaultAsync();
                if (category is null)
                    return NotFound($"Category with Id = {id} not found.");

                return Ok(category);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto cate)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var entity = new Category
                {
                    Name = cate.Name
                };

                await _dbContext.Categories.AddAsync(entity);
                await _dbContext.SaveChangesAsync();

                var result = new CategoryDto(entity.Id, entity.Name);
                return CreatedAtAction(
                    nameof(GetCategoryById),
                    new { id = entity.Id },
                    result
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryDto cate)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var existingCategory = await _dbContext.Categories.FindAsync(id);
                if (existingCategory is null)
                    return NotFound($"Category with Id = {id} not found.");

                existingCategory.Name = cate.Name;
                await _dbContext.SaveChangesAsync();

                var result = new CategoryDto(existingCategory.Id, existingCategory.Name);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var existingCategory = await _dbContext.Categories.FindAsync(id);
                if (existingCategory is null)
                    return NotFound($"Category with Id = {id} not found.");

                _dbContext.Categories.Remove(existingCategory);
                await _dbContext.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
