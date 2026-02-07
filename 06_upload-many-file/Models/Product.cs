using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _06_upload_many_file.Models;

public class Product
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required(ErrorMessage = "Name product is required.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Price product is required.")]
    [Column(TypeName = "decimal(10,2)")]
    public double Price { get; set; }

    public List<ProductImage>? ProductImages { get; set; } = new();
}