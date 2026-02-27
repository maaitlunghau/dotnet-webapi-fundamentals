using backend.Data;
using backend.Repositories;
using Microsoft.EntityFrameworkCore;
using Shared.Domain;

namespace backend.Services;

public class RefreshTokenService : IRefreshTokenRepository
{
    private readonly DataContext _dbContext;
    public RefreshTokenService(DataContext context) => _dbContext = context;

    public async Task<RefreshTokenRecord?> GetByAccessTokenJtiAsync(string jti)
    {
        return await _dbContext.RefreshTokenRecords
            .FirstOrDefaultAsync(rt => rt.AccessTokenJti == jti);
    }

    public async Task<RefreshTokenRecord?> GetByRefreshTokenAsync(string refreshToken)
    {
        return await _dbContext.RefreshTokenRecords
            .FirstOrDefaultAsync(rt => rt.RefreshToken == refreshToken);
    }

    public async Task<RefreshTokenRecord> CreateAsync(RefreshTokenRecord refreshToken)
    {
        await _dbContext.RefreshTokenRecords.AddAsync(refreshToken);
        await _dbContext.SaveChangesAsync();

        return refreshToken;
    }

    public async Task RevokeAsync(RefreshTokenRecord refreshToken)
    {
        refreshToken.RevokedAtUTC = DateTime.UtcNow;

        _dbContext.RefreshTokenRecords.Update(refreshToken);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<int> GetActiveTokenCountByUserIdAsync(Guid userId)
    {
        var now = DateTime.UtcNow;

        return await _dbContext.RefreshTokenRecords
            .CountAsync(rt =>
                rt.UserId == userId &&
                rt.RevokedAtUTC == null &&
                rt.ExpireAtUTC > now
            );
    }
}
