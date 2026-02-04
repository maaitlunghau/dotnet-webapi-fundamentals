using System.Net;
using _04_many_to_many.DTOs;
using _04_many_to_many.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _04_many_to_many.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly DataContext _dbContext;
        public CourseController(DataContext dbContext) => _dbContext = dbContext;


        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var courses = await _dbContext.Courses
                    .Include(c => c.StudentCourses!)
                    .ThenInclude(sc => sc.Student)
                    .Select(c => new CourseDto
                    (
                        c.Id,
                        c.Title,
                        c.Price,
                        c.Status,
                        c.StudentCourses!.Select(sc => new StudentEnrollmentDto(
                            sc.StudentId,
                            sc.Student!.Name,
                            sc.Student.Age,
                            sc.EnrollDate
                        )).ToList()
                    ))
                    .ToListAsync();

                return Ok(courses);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetSingleCourse(Guid id)
        {
            try
            {
                var course = await _dbContext.Courses
                    .Include(c => c.StudentCourses!)
                    .ThenInclude(sc => sc.Student)
                    .Where(c => c.Id == id)
                    .Select(c => new CourseDto
                    (
                        c.Id,
                        c.Title,
                        c.Price,
                        c.Status,
                        c.StudentCourses!.Select(sc => new StudentEnrollmentDto(
                            sc.StudentId,
                            sc.Student!.Name,
                            sc.Student.Age,
                            sc.EnrollDate
                        )).ToList()
                    ))
                    .FirstOrDefaultAsync();
                if (course is null) return NotFound("Không tìm thấy khoá học này!");

                return Ok(course);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        [HttpPost]
        public async Task<IActionResult> Create(CreateCourseDto course)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var entity = new Course
                {
                    Title = course.Title,
                    Price = course.Price,
                    Status = course.Status
                };

                await _dbContext.Courses.AddAsync(entity);
                await _dbContext.SaveChangesAsync();

                var result = new CourseDto
                (
                    entity.Id,
                    entity.Title,
                    entity.Price,
                    entity.Status,
                    new List<StudentEnrollmentDto>()
                );

                return CreatedAtAction(
                    nameof(GetSingleCourse),
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
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCourseDto course)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var existingCourse = await _dbContext.Courses.FindAsync(id);
                if (existingCourse is null) return NotFound("Khoá học không tồn tại!");

                existingCourse.Title = course.Title;
                existingCourse.Price = course.Price;
                existingCourse.Status = course.Status;

                await _dbContext.SaveChangesAsync();

                var result = new CourseDto(
                    existingCourse.Id,
                    existingCourse.Title,
                    existingCourse.Price,
                    existingCourse.Status,
                    new List<StudentEnrollmentDto>()
                );

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var existingCourse = await _dbContext.Courses.FindAsync(id);
                if (existingCourse is null) return NotFound("Khoá học không tồn tại!");

                _dbContext.Courses.Remove(existingCourse);
                await _dbContext.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        [HttpPost("{courseId:Guid}/student/{studentId:Guid}")]
        public async Task<IActionResult> EnrollStudent(Guid courseId, Guid studentId)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var exisitingCourse = await _dbContext.Courses.FindAsync(courseId);
                if (exisitingCourse is null) return NotFound("Khoá học không tồn tại!");

                var existingStudent = await _dbContext.Students.FindAsync(studentId);
                if (existingStudent is null) return NotFound("Sinh viên không tồn tại!");

                var existingEnrollment = await _dbContext.StudentCourses
                    .FirstOrDefaultAsync(sc => sc.CourseId == courseId && sc.StudentId == studentId);
                if (existingEnrollment is not null) return BadRequest("Sinh viên đã đăng ký khoá học này rồi!");

                var enrollment = new StudentCourse
                {
                    StudentId = studentId,
                    CourseId = courseId,
                    EnrollDate = DateTime.Now
                };

                await _dbContext.StudentCourses.AddAsync(enrollment);
                await _dbContext.SaveChangesAsync();

                var result = new
                {
                    StudentId = studentId,
                    CourseId = courseId,
                    EnrollDate = enrollment.EnrollDate,
                    Message = "Đăng ký thành công"
                };

                return CreatedAtAction(
                    nameof(GetSingleCourse),
                    new { id = courseId },
                    result
                );
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        [HttpDelete("{courseId:Guid}/student/{studentId:Guid}")]
        public async Task<IActionResult> RemoveStudent(Guid courseId, Guid studentId)
        {
            try
            {
                var exisitingCourse = await _dbContext.Courses.FindAsync(courseId);
                if (exisitingCourse is null) return NotFound("Khoá học không tồn tại!");

                var existingStudent = await _dbContext.Students.FindAsync(studentId);
                if (existingStudent is null) return NotFound("Sinh viên không tồn tại!");

                var existingEnrollment = await _dbContext.StudentCourses
                    .FirstOrDefaultAsync(sc => sc.CourseId == courseId && sc.StudentId == studentId);
                if (existingEnrollment is null) return NotFound("Không tìm thấy đăng ký này");

                _dbContext.StudentCourses.Remove(existingEnrollment);
                await _dbContext.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        // Batch Operations
        // [HttpPost("{courseId:Guid}/students")]
        // public async Task<IActionResult> EnrollMultipleStudents(Guid courseId, [FromBody] List<Guid> studentIds)
        // {
        //     if (studentIds == null || !studentIds.Any())
        //         return BadRequest("Danh sách sinh viên không được rỗng!");

        //     try
        //     {
        //         // Check if course exists
        //         var course = await _dbContext.Courses.FindAsync(courseId);
        //         if (course is null) return NotFound("Khoá học không tồn tại!");

        //         var results = new List<object>();
        //         var successCount = 0;
        //         var failedCount = 0;

        //         foreach (var studentId in studentIds)
        //         {
        //             // Check if student exists
        //             var student = await _dbContext.Students.FindAsync(studentId);
        //             if (student is null)
        //             {
        //                 results.Add(new { StudentId = studentId, Status = "Failed", Message = "Sinh viên không tồn tại" });
        //                 failedCount++;
        //                 continue;
        //             }

        //             // Check if already enrolled
        //             var existingEnrollment = await _dbContext.StudentCourses
        //                 .FirstOrDefaultAsync(sc => sc.CourseId == courseId && sc.StudentId == studentId);

        //             if (existingEnrollment != null)
        //             {
        //                 results.Add(new { StudentId = studentId, StudentName = student.Name, Status = "Skipped", Message = "Đã đăng ký rồi" });
        //                 failedCount++;
        //                 continue;
        //             }

        //             // Create enrollment
        //             var enrollment = new StudentCourse
        //             {
        //                 CourseId = courseId,
        //                 StudentId = studentId,
        //                 EnrollDate = DateTime.Now
        //             };

        //             await _dbContext.StudentCourses.AddAsync(enrollment);
        //             results.Add(new { StudentId = studentId, StudentName = student.Name, Status = "Success", EnrollDate = enrollment.EnrollDate });
        //             successCount++;
        //         }

        //         await _dbContext.SaveChangesAsync();

        //         return Ok(new
        //         {
        //             Message = $"Đăng ký thành công {successCount}/{studentIds.Count} sinh viên",
        //             CourseTitle = course.Title,
        //             SuccessCount = successCount,
        //             FailedCount = failedCount,
        //             Details = results
        //         });
        //     }
        //     catch (Exception ex)
        //     {
        //         return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
        //     }
        // }


        // [HttpDelete("{courseId:Guid}/students")]
        // public async Task<IActionResult> RemoveMultipleStudents(Guid courseId, [FromBody] List<Guid> studentIds)
        // {
        //     if (studentIds == null || !studentIds.Any())
        //         return BadRequest("Danh sách sinh viên không được rỗng!");

        //     try
        //     {
        //         // Check if course exists
        //         var course = await _dbContext.Courses.FindAsync(courseId);
        //         if (course is null) return NotFound("Khoá học không tồn tại!");

        //         var results = new List<object>();
        //         var successCount = 0;
        //         var failedCount = 0;

        //         foreach (var studentId in studentIds)
        //         {
        //             // Find enrollment
        //             var enrollment = await _dbContext.StudentCourses
        //                 .FirstOrDefaultAsync(sc => sc.CourseId == courseId && sc.StudentId == studentId);

        //             if (enrollment is null)
        //             {
        //                 results.Add(new { StudentId = studentId, Status = "Failed", Message = "Không tìm thấy đăng ký" });
        //                 failedCount++;
        //                 continue;
        //             }

        //             // Remove enrollment
        //             _dbContext.StudentCourses.Remove(enrollment);
        //             results.Add(new { StudentId = studentId, Status = "Success" });
        //             successCount++;
        //         }

        //         await _dbContext.SaveChangesAsync();

        //         return Ok(new
        //         {
        //             Message = $"Xoá thành công {successCount}/{studentIds.Count} sinh viên",
        //             CourseTitle = course.Title,
        //             SuccessCount = successCount,
        //             FailedCount = failedCount,
        //             Details = results
        //         });
        //     }
        //     catch (Exception ex)
        //     {
        //         return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
        //     }
        // }
    }
}
