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
        public async Task<ActionResult<List<Course>>> GetStudents()
        {
            var students = await _dbContext.Students.Select(s => new
            {
                s.Id,
                s.Name,
                s.Age,
                CoursesXX = s.StudentCourse.Select(sc => new
                {
                    sc.CourseId,
                    sc.Course!.Title,
                    sc.EnrollDate
                }).ToList()
            }).ToListAsync();
            return Ok(students);
        }

        [HttpPost]
        public async Task<IActionResult> AddCourse(Student student)
        {
            await _dbContext.Students.AddAsync(student);
            await _dbContext.SaveChangesAsync();

            return Created("", student);
        }

        [HttpPost("{id:int}/course")]
        public async Task<IActionResult> EnrollStudent(int id, int courseId)
        {
            var studentExisting = await _dbContext.Students.AnyAsync(s => s.Id == id);
            var courseExisting = await _dbContext.Courses.AnyAsync(c => c.Id == courseId);
            if (!studentExisting || !courseExisting)
                return NotFound("Student or Course not found");

            var exists = await _dbContext.StudentCourses
                .AnyAsync(sc => sc.CourseId == courseId && sc.StudentId == id);

            if (exists)
                return Conflict("Already Enrolled");

            await _dbContext.StudentCourses.AddAsync(new StudentCourse()
            {
                CourseId = courseId,
                StudentId = id
            });

            await _dbContext.SaveChangesAsync();
            return Ok("Enrolled successfully");
        }
    }
}