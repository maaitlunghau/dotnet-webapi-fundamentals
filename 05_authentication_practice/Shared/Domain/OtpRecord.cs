using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Domain;

public class OtpRecord
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid UserId { get; set; }
    // nhận biết người dùng nào sở hữu OTP này

    [Required]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "OTP must be 6 digits")]
    public string OtpCode { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime ExpireAtUtc { get; set; }

    public bool IsUsed { get; set; } = false;

    public DateTime? UsedAtUtc { get; set; }

    [NotMapped]
    public bool IsExpired => DateTime.UtcNow >= ExpireAtUtc;

    [NotMapped]
    public bool IsActive => !IsUsed && !IsExpired;

    // navigation property
    public User? User { get; set; }
}