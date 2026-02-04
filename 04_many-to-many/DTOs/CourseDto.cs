using _04_many_to_many.Models;

namespace _04_many_to_many.DTOs;

public record CourseDto(
    Guid Id,
    string Title,
    decimal Price,
    bool Status,
    List<StudentCourse> StudentCourses
);

public record CreateCourseDto(
    string Title,
    decimal Price,
    bool Status,
    List<StudentCourse>? StudentCourse
);

public record UpdateCourseDto(
    string Title,
    decimal Price,
    bool Status,
    List<StudentCourse>? StudentCourse
);