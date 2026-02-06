namespace frontend.DTOs;

public record AuthResponseDto(
    string AccessToken,
    string RefreshToken
);
