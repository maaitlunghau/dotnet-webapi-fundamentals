namespace _04_backend.Models;

public class Student
{
    public int Id { get; set; }
    public string? Name { get; set; } = default;
    public int Age { get; set; }
    public bool Status { get; set; }
    public ICollection<StudentCourse> StudentCourse { get; set; } = new List<StudentCourse>();
}