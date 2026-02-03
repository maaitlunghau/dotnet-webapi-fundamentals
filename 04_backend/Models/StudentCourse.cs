namespace _04_backend.Models;

public class StudentCourse
{
    public int StudentId { get; set; }
    public int CourseId { get; set; }

    // relationship
    public Student? Student { get; set; }
    public Course? Course { get; set; }

    public DateTime EnrollDate { get; set; } = DateTime.Now;
}
