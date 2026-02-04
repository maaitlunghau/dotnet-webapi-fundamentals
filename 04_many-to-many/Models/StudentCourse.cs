namespace _04_many_to_many.Models;

public class StudentCourse
{
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    public DateTime EnrollDate { get; set; } = DateTime.Now;

    // navigation properties
    public Student? Student { get; set; }
    public Course? Course { get; set; }
}
