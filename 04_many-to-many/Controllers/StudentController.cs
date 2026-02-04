using System.Net;
using _04_many_to_many.DTOs;
using _04_many_to_many.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _04_many_to_many.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly DataContext _dbContext;
        public StudentController(DataContext dbContext) => _dbContext = dbContext;

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var students = await _dbContext.Students
                    .Include(sc => sc.StudentCourses!)
                    .ThenInclude(c => c.Course)
                    .Select(s => new StudentDto(
                        s.Id,
                        s.Name,
                        s.Age,
                        s.Status,
                        s.StudentCourses!.Select(sc => new CourseEnrollmentDto(
                            sc.CourseId,
                            sc.Course!.Title,
                            sc.Course.Price,
                            sc.EnrollDate
                        )).ToList()
                    )).ToListAsync();

                return Ok(students);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetSingleStudent(Guid id)
        {
            try
            {
                var students = await _dbContext.Students
                    .Include(sc => sc.StudentCourses!)
                    .ThenInclude(c => c.Course)
                    .Where(s => s.Id == id)
                    .Select(s => new StudentDto(
                        s.Id,
                        s.Name,
                        s.Age,
                        s.Status,
                        s.StudentCourses!.Select(sc => new CourseEnrollmentDto(
                            sc.CourseId,
                            sc.Course!.Title,
                            sc.Course.Price,
                            sc.EnrollDate
                        )).ToList()
                    )).FirstOrDefaultAsync();

                return Ok(students);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        [HttpPost]
        public async Task<IActionResult> CreateStudent([FromBody] CreateStudentDto stu)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var entity = new Student
                {
                    Name = stu.Name,
                    Age = stu.Age,
                    Status = stu.Status
                };

                await _dbContext.Students.AddAsync(entity);
                await _dbContext.SaveChangesAsync();

                var result = new StudentDto(
                    entity.Id,
                    entity.Name,
                    entity.Age,
                    entity.Status,
                    new List<CourseEnrollmentDto>()
                );

                return CreatedAtAction(
                    nameof(GetSingleStudent),
                    new { id = entity.Id },
                    result
                );
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> UpdateStudent(Guid id, [FromBody] UpdateStudentDto stu)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var existingStudent = await _dbContext.Students.FindAsync(id);
                if (existingStudent is null)
                    return NotFound($"Sinh viên không tồn tại!");

                existingStudent.Name = stu.Name;
                existingStudent.Age = stu.Age;
                existingStudent.Status = stu.Status;

                await _dbContext.SaveChangesAsync();

                var result = new StudentDto(
                    existingStudent.Id,
                    existingStudent.Name,
                    existingStudent.Age,
                    existingStudent.Status,
                    new List<CourseEnrollmentDto>()
                );

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> DeleteStudent(Guid id)
        {
            try
            {
                var existingStudent = await _dbContext.Students.FindAsync(id);
                if (existingStudent is null)
                    return NotFound($"Could not find student with Id = {id}");

                _dbContext.Students.Remove(existingStudent);
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