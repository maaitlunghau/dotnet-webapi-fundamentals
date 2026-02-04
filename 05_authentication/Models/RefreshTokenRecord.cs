namespace _05_authentication.Models;

public class RefreshTokenRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid AccountId { get; set; }

    public string RefreshToken { get; set; }

    public string AccessTokenJti { get; set; }

    public DateTime ExpireAtUtc { get; set; }

    public string? ReplaceByToken { get; set; }

    public DateTime? RevokeAtUtc { get; set; }

    public bool IsExpired => DateTime.UtcNow >= ExpireAtUtc;

    public bool IsActive => RevokeAtUtc == null && !IsExpired;

    // navigation property
    public Account? Account { get; set; }
}