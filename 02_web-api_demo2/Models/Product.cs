using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _02_web_api_demo2.Models;

public class Product
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int CaetgoryId { get; set; }

    [Required(ErrorMessage = "Tên sản phẩm không được bỏ trống!")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Độ dài tên sản phẩm phải từ 3-100 ký tự!")]
    public string Name { get; set; } = string.Empty;

    [Column(TypeName = "decimal(10,2)")]
    [Required(ErrorMessage = "Giá sản phẩm không được bỏ trống!")]

    public decimal Price { get; set; }

    [Required(ErrorMessage = "Số lượng sản phẩm không được bỏ trống!")]
    public int Stock { get; set; }

    // navigation property
    public Category? Category { get; set; }
}
