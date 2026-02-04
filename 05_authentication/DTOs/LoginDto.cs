namespace _05_authentication.DTOs;

public record LoginRequestDto(
    string Email,
    string Password
);