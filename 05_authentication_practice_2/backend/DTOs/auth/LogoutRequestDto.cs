using System;

namespace backend.DTOs.auth;

public class LogoutRequestDto
{
    public string? RefreshToken { get; set; } = string.Empty;
}
