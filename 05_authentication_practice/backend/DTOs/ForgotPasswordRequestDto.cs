using System.ComponentModel.DataAnnotations;

namespace backend.DTOs;

public record ForgotPasswordRequestDto(
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    string Email
);