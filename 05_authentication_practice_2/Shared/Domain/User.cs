using System.ComponentModel.DataAnnotations;

namespace Shared.Domain;

public class User
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required(ErrorMessage = "Name is required")]
    [StringLength(50, ErrorMessage = "Name must be less than 50 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email is invalid")]
    [StringLength(100, ErrorMessage = "Email must be less than 100 characters")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Role { get; set; } = "user";

    public DateTime CreatedAtUTC { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAtUTC { get; set; } = DateTime.UtcNow;

    // navigation property
    public ICollection<RefreshTokenRecord>? RefreshTokenRecords { get; set; }
}
