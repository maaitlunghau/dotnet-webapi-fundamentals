namespace _04_many_to_many.DTOs;

public record CourseDto(
    Guid Id,
    string Title,
    decimal Price,
    bool Status,
    List<StudentEnrollmentDto> Students
);

public record CreateCourseDto(
    string Title,
    decimal Price,
    bool Status
);

public record UpdateCourseDto(
    string Title,
    decimal Price,
    bool Status
);

public record StudentEnrollmentDto(
    Guid StudentId,
    string StudentName,
    int Age,
    DateTime EnrollDate
);