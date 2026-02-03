using backend.Data;
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
                var categories = await _dbContext.Categories.ToListAsync();
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
                var category = await _dbContext.Categories.FindAsync(id);
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
        public async Task<IActionResult> CreateCategory([FromBody] Category category)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await _dbContext.Categories.AddAsync(category);
                await _dbContext.SaveChangesAsync();

                return Created("Created new category successfully", category);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] Category category)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var existingCategory = await _dbContext.Categories.FindAsync(id);
                if (existingCategory is null)
                    return NotFound($"Category with Id = {id} not found.");

                existingCategory.Name = category.Name;
                await _dbContext.SaveChangesAsync();

                return Ok(existingCategory);
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
