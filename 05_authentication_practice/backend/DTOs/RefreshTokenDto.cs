using System.ComponentModel.DataAnnotations;

namespace backend.DTOs;

public record RefreshTokenDto(
    [Required(ErrorMessage = "Refresh token is required")]
    string RefreshToken
);