using System.ComponentModel.DataAnnotations;

namespace _04_many_to_many.Models;

public class Student
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required(ErrorMessage = "Tên sinh viên không được bỏ trống!")]
    [StringLength(50, ErrorMessage = "Tên sinh viên không được quá 50 ký tự!")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tuổi sinh viên không được bỏ trống!")]
    [Range(18, 100, ErrorMessage = "Tuổi sinh viên phải từ 18 đến 100!")]
    public int Age { get; set; }

    [Required(ErrorMessage = "Trạng thái sinh viên không được bỏ trống!")]
    public bool Status { get; set; } = true;

    // navigation property
    public ICollection<StudentCourse>? StudentCourses { get; set; } = new List<StudentCourse>();
}