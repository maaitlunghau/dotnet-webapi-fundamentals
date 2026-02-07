using System.ComponentModel.DataAnnotations;

namespace _06_upload_many_file.DTOs;

public class ProductCreateRequestDto
{
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Price product is required.")]
    public double Price { get; set; }

    public List<IFormFile>? Images { get; set; } = new List<IFormFile>();
}
