using System.ComponentModel.DataAnnotations;

namespace frontend.DTOs;

public class VerifyOtpRequestDto
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "OTP is required")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "OTP must be 6 digits")]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "OTP must be 6 digits")]
    public string OtpCode { get; set; } = string.Empty;
}
