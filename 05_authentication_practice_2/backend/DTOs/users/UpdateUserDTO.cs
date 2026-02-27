using System.ComponentModel.DataAnnotations;

namespace backend.DTOs;

public class UpdateUserDTO
{
    [StringLength(50, ErrorMessage = "Name must be less than 50 characters")]
    public string? Name { get; set; }

    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(100, ErrorMessage = "Email must be less than 100 characters")]
    public string? Email { get; set; }
}
