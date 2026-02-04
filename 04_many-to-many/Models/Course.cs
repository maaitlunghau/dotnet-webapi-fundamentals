using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _04_many_to_many.Models;

public class Course
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required(ErrorMessage = "Tên khoá học không được phép bỏ trống!")]
    [StringLength(255, MinimumLength = 6, ErrorMessage = "Tiêu đề khoá học phải từ 6-255 ký tự!")]
    public string Title { get; set; } = string.Empty;

    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Trạng thái khoá học không được phép bỏ trống!")]
    public bool Status { get; set; } = true;

    // navigation property
    public ICollection<StudentCourse>? StudentCourses { get; set; } = new List<StudentCourse>();
}
