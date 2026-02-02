using _04_backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _04_backend.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly DataContext _dbContext;
        public StudentController(DataContext dbContext) => _dbContext = dbContext;

        [HttpGet]
        public async Task<ActionResult<List<Course>>> Get()
        {
            var students = await _dbContext.Students.ToListAsync();
            return Ok(students);
        }

        [HttpPost]
        public async Task<IActionResult> AddCourse(Student student)
        {
            await _dbContext.Students.AddAsync(student);
            await _dbContext.SaveChangesAsync();

            return Created("", student);
        }
    }
}
