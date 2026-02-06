using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _06_upload_many_file.Models;

public class ProductImage
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int ProductId { get; set; }

    [Required(ErrorMessage = "Url is required.")]
    public int Url { get; set; }

    public Product? Product { get; set; }
}
