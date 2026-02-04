using _04_many_to_many.Models;

namespace _04_many_to_many.DTOs;

public record StudentDto(
    Guid Id,
    string Name,
    int Age,
    bool Status,
    List<StudentCourse> StudentCourses
);

public record CreateStudentDto(
    string Name,
    int Age,
    bool Status
);

public record UpdateStudentDto(
    string Name,
    int Age,
    bool Status,
    List<StudentCourse>? StudentCourses
);