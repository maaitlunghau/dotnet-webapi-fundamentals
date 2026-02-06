namespace backend.DTOs;

public record AuthResponseDto(
    string AccessToken,
    string RefreshToken
);
