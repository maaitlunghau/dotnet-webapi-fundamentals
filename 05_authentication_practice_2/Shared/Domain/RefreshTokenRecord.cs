using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Domain;

public class RefreshTokenRecord
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid UserId { get; set; }

    [Required]
    [StringLength(500)]
    public string RefreshToken { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string AccessTokenJti { get; set; } = string.Empty;

    [StringLength(500)]
    public string? ReplacedByRefreshToken { get; set; }

    public DateTime? RevokedAtUTC { get; set; }

    public DateTime CreatedAtUTC { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime ExpireAtUTC { get; set; }

    [NotMapped]
    public bool IsExpired => DateTime.UtcNow >= ExpireAtUTC;

    [NotMapped]
    public bool IsActive => RevokedAtUTC == null && !IsExpired;

    // Navigation property
    public User? User { get; set; }
}
