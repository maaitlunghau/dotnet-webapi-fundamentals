using System.Net;
using backend.DTOs.auth;
using backend.Repositories;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly TokenService _tokenService;

        public AuthController(
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            TokenService tokenService
        )
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var existingUser = await _userRepository.GetUserByEmailAsync(dto.Email);
                if (existingUser == null || !BCrypt.Net.BCrypt.Verify(dto.Password, existingUser.Password))
                    return Unauthorized(new { message = "Invalid email or password" });

                // check max device limit (5 devices)
                const int maxDevices = 5;

                var activeTokenCount = await _refreshTokenRepository.GetActiveTokenCountByUserIdAsync(existingUser.Id);
                if (activeTokenCount >= maxDevices)
                    return BadRequest(new
                    {
                        message = $"Maximum {maxDevices} devices allowed. Please logout from another device."
                    }
                );

                // generate access and refresh tokens
                var (accessToken, jti) = _tokenService.CreateAccessToken(existingUser);
                var refreshTokenRecord = _tokenService.CreateRefreshToken(existingUser.Id, jti);

                await _refreshTokenRepository.CreateAsync(refreshTokenRecord);

                return Ok(new LoginResponseDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshTokenRecord.RefreshToken
                });
            }
            catch (Exception ex)
            {
                var detailMessage = $"Exception message: {ex.Message}. Detail: {ex}";
                return StatusCode((int)HttpStatusCode.InternalServerError, detailMessage);
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var refreshToken = await _refreshTokenRepository.GetByRefreshTokenAsync(dto.RefreshToken!);
                if (refreshToken is null)
                    return NotFound(new { message = "Refresh token not found" });

                if (!refreshToken.IsActive)
                    return Ok(new { message = "Already logged out" });

                await _refreshTokenRepository.RevokeAsync(refreshToken);

                return Ok(new { message = "Logged out successfully" });
            }
            catch (Exception ex)
            {
                var detailMessage = $"Exception message: {ex.Message}. Detail: {ex}";
                return StatusCode((int)HttpStatusCode.InternalServerError, detailMessage);
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // Validate refresh token
                var currentRefreshToken = await _refreshTokenRepository.GetByRefreshTokenAsync(dto.RefreshToken!);
                if (currentRefreshToken is null)
                    return Unauthorized(new { message = "Invalid refresh token" });

                if (!currentRefreshToken.IsActive)
                    return Unauthorized(new { message = "Refresh token has been revoked or expired" });

                // Get user
                var user = await _userRepository.GetUserByIdAsync(currentRefreshToken.UserId);
                if (user is null)
                    return Unauthorized(new { message = "User not found" });

                // Generate new tokens
                var (newAccessToken, newJti) = _tokenService.CreateAccessToken(user);
                var newRefreshTokenRecord = _tokenService.CreateRefreshToken(user.Id, newJti);

                // Save new refresh token
                await _refreshTokenRepository.CreateAsync(newRefreshTokenRecord);

                // Revoke old token and track replacement
                currentRefreshToken.ReplacedByRefreshToken = newRefreshTokenRecord.RefreshToken;
                await _refreshTokenRepository.RevokeAsync(currentRefreshToken);

                return Ok(new RefreshResponseDto
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshTokenRecord.RefreshToken
                });
            }
            catch (Exception ex)
            {
                var detailMessage = $"Exception message: {ex.Message}. Detail: {ex}";
                return StatusCode((int)HttpStatusCode.InternalServerError, detailMessage);
            }
        }
    }
}
