using System.ComponentModel.DataAnnotations;

namespace backend.DTOs;

public record LogoutRequestDto(
    [Required(ErrorMessage = "Refresh token is required")]
    string refreshToken
);