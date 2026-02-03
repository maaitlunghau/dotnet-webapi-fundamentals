using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _01_web_api_demo.Models;

public class Product
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required(ErrorMessage = "Tên sản phẩm không được bỏ trống!")]
    [StringLength(40, ErrorMessage = "Tên sản phẩm không được quá 40 ký tự")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Giá tiền sản phẩm không được bỏ trống!")]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Số lượng tồn kho không được bỏ trống!")]
    [Range(1, int.MaxValue, ErrorMessage = "Số lượng tồn kho phải từ 1 đến Max Integer")]
    public int Stock { get; set; }

    [Required(ErrorMessage = "Trạng thái sản phẩm không được bỏ trống!")]
    public bool IsActive { get; set; } = true;

    [StringLength(200, ErrorMessage = "Mô tả sản phẩm không được quá 200 ký tự")]
    public string? Description { get; set; }
}
