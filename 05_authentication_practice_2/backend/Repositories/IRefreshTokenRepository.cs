using Shared.Domain;

namespace backend.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshTokenRecord?> GetByAccessTokenJtiAsync(string jti);
    Task<RefreshTokenRecord?> GetByRefreshTokenAsync(string refreshToken);
    Task<RefreshTokenRecord> CreateAsync(RefreshTokenRecord refreshToken);
    Task RevokeAsync(RefreshTokenRecord refreshToken);
    Task<int> GetActiveTokenCountByUserIdAsync(Guid userId);
}
