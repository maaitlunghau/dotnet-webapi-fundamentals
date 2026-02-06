using System.ComponentModel.DataAnnotations;

namespace backend.DTOs;

public record ResetPasswordRequestDto(
    [Required][EmailAddress]
    string Email,

    [Required][StringLength(6, MinimumLength = 6)]
    string Otp,

    [Required][StringLength(100, MinimumLength = 8)]
    string NewPassword
);