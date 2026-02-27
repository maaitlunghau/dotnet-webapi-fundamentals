using System;

namespace backend.DTOs.auth;

public class RefreshRequestDto
{
    public string? RefreshToken { get; set; } = string.Empty;
}
