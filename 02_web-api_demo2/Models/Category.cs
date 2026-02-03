using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _02_web_api_demo2.Models;

public class Category
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required(ErrorMessage = "Tên loại sản phẩm không được bỏ trống!")]
    public string Name { get; set; } = string.Empty;

    // navigation property
    public ICollection<Product>? Products { get; set; }
}
