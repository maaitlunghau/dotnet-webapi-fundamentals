namespace _05_authentication.DTOs;

public record AuthResponseDto(
    string accessToken,
    string refreshToken
);