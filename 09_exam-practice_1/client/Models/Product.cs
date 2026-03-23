using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace client.Models;

public class Product
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
    [StringLength(10, MinimumLength = 3, ErrorMessage = "Tên sản phẩm phải từ 3 đến 10 ký tự")]
    [Column(TypeName = "varchar(100)")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Giá là bắt buộc")]
    [Range(100, 400, ErrorMessage = "Giá phải nằm trong khoảng từ 100 đến 400")]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }

    [Required]
    public bool Status { get; set; } = true;

    [Required(ErrorMessage = "Hình ảnh là bắt buộc")]
    [MaxLength(255, ErrorMessage = "Đường dẫn hình ảnh không quá 255 ký tự")]
    [Column(TypeName = "varchar(255)")]
    public string Image { get; set; } = string.Empty;
}
