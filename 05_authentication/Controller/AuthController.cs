using _05_authentication.DTOs;
using _05_authentication.Models;
using _05_authentication.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _05_authentication.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DataContext _dbContext;
        private readonly TokenService _tokenService;
        public AuthController(DataContext context, TokenService tokenService)
        {
            _dbContext = context;
            _tokenService = tokenService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequestDto loginRequest)
        {
            var acc = await _dbContext.Accounts.FirstOrDefaultAsync(a =>
                a.Email == loginRequest.Email &&
                a.Password == loginRequest.Password
            );
            if (acc is null) return Unauthorized();

            var (accessToken, jti) = _tokenService.CreateAccessToken(acc);
            var frToken = _tokenService.CreateRefreshtoken(acc.Id, jti);

            await _dbContext.RefreshTokenRecords.AddAsync(frToken);
            await _dbContext.SaveChangesAsync();

            return Ok(
                new AuthResponseDto(
                    accessToken,
                    frToken.RefreshToken
                )
            );
        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequestDto refreshToken)
        {
            var oldToken = await _dbContext.RefreshTokenRecords.FirstOrDefaultAsync(o
                => o.RefreshToken == refreshToken.refreshToken
            );
            if (oldToken is null || !oldToken.IsActive)
                return Unauthorized("Refresh token không hợp lệ!");

            var acc = await _dbContext.Accounts
                .FirstOrDefaultAsync(x => x.Id == oldToken.AccountId);
            if (acc is null) return Unauthorized("Refresh token không hợp lệ");

            oldToken.RevokeAtUtc = DateTime.UtcNow;

            var (accessToken, jti) = _tokenService.CreateAccessToken(acc);
            var newRefreshToken = _tokenService.CreateRefreshtoken(acc.Id, jti);

            oldToken.ReplaceByToken = newRefreshToken.RefreshToken;

            await _dbContext.RefreshTokenRecords.AddAsync(newRefreshToken);
            await _dbContext.SaveChangesAsync();

            return Ok(
                new AuthResponseDto(
                    accessToken,
                    newRefreshToken.RefreshToken
                )
            );
        }
    }
}