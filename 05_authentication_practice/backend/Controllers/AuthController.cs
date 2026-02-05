using backend.Data;
using backend.DTOs;
using backend.Service;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Domain;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DataContext _dbContext;
        private readonly TokenService _tokenService;
        private readonly OtpService _otpService;
        public AuthController(DataContext dbContext, TokenService tokenService, OtpService otpService)
        {
            _dbContext = dbContext;
            _tokenService = tokenService;
            _otpService = otpService;
        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u =>
                u.Email == loginRequest.Email
            );
            if (existingUser is null)
                return Unauthorized("Invalid email or password (1)");

            if (!BCrypt.Net.BCrypt.Verify(loginRequest.Password, existingUser.Password))
                return Unauthorized("Invalid email or password (2)");

            var oldestActiveToken = null as RefreshTokenRecord;
            int maxDevices = 3;

            // count the active tokens
            var activeTokenCount = await _dbContext.RefreshTokenRecords.CountAsync(rft =>
                rft.UserId == existingUser.Id && rft.RevokeAtUtc == null && rft.ReplacedByRefreshToken == null
            );
            if (activeTokenCount >= maxDevices)
            {
                // get the oldest active token (by creation time)
                oldestActiveToken = await _dbContext.RefreshTokenRecords
                    .Where(rft => rft.UserId == existingUser.Id &&
                           rft.RevokeAtUtc == null &&
                           rft.ReplacedByRefreshToken == null)
                    .OrderBy(rft => rft.CreatedAtUtc)
                    .FirstAsync();

                // revoke oldest token
                oldestActiveToken.RevokeAtUtc = DateTime.UtcNow;
            }

            // generate tokens
            var (accessToken, jti) = _tokenService.CreateAccessToken(existingUser);

            // generate refresh token
            var refreshToken = _tokenService.CreateRefreshToken(existingUser.Id, jti);
            await _dbContext.RefreshTokenRecords.AddAsync(refreshToken);

            // replace oldest token by new token
            if (oldestActiveToken is not null)
                oldestActiveToken.ReplacedByRefreshToken = refreshToken.RefreshToken;

            await _dbContext.SaveChangesAsync();

            return Ok(new AuthResponseDto(
                AccessToken: accessToken,
                RefreshToken: refreshToken.RefreshToken
            ));
        }


        [HttpPost("Logout")]
        public async Task<IActionResult> Logout(LogoutRequestDto logoutRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var refreshToken = await _dbContext.RefreshTokenRecords.FirstOrDefaultAsync(rft =>
                rft.RefreshToken == logoutRequest.refreshToken
            );
            if (refreshToken is null || !refreshToken.IsActive)
                return Unauthorized("Invalid refresh token");

            refreshToken.RevokeAtUtc = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            return Ok("Logged out successfully");
        }


        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken(RefreshTokenDto refreshToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // tìm refresh token cũ
            var oldRefreshToken = await _dbContext.RefreshTokenRecords.FirstOrDefaultAsync(rft =>
                rft.RefreshToken == refreshToken.RefreshToken
            );
            if (oldRefreshToken is null || !oldRefreshToken.IsActive)
                return Unauthorized("Invalid refresh token");

            // tìm user nào sở hữu refresh token đó
            var user = await _dbContext.Users.FirstOrDefaultAsync(u =>
                u.Id == oldRefreshToken.UserId
            );
            if (user is null) return Unauthorized("User not found");

            // cập nhật thời gian RevokeAtUtc cho refresh token cũ = thời gian hiện tại
            oldRefreshToken.RevokeAtUtc = DateTime.UtcNow;

            // tạo access token mới
            var (accessToken, jwi) = _tokenService.CreateAccessToken(user);

            // tạo refresh token mới
            var newRefreshToken = _tokenService.CreateRefreshToken(user.Id, jwi);
            await _dbContext.RefreshTokenRecords.AddAsync(newRefreshToken);

            // cập nhật ReplacByToken cho refresh token cũ = giá trị của refresh token mới
            oldRefreshToken.ReplacedByRefreshToken = newRefreshToken.RefreshToken;

            // cập nhật cơ sở dữ liệu
            await _dbContext.SaveChangesAsync();

            // trả về access token mới và refresh token mới            
            return Ok(new AuthResponseDto(
                AccessToken: accessToken,
                RefreshToken: newRefreshToken.RefreshToken
            ));
        }


        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u =>
                u.Email == request.Email
            );
            if (existingUser is null)
                return Ok("If the email is registered, an OTP has been sent.");

            // xoá toàn bộ OTP cũ chưa sử dụng
            var oldOtps = await _dbContext.OtpRecords
                .Where(otp => otp.UserId == existingUser.Id && !otp.IsUsed)
                .ToListAsync();
            if (oldOtps.Any()) _dbContext.OtpRecords.RemoveRange(oldOtps);

            // tạo OTP mới
            var otpRecord = _otpService.CreateOtp(existingUser.Id);
            await _dbContext.OtpRecords.AddAsync(otpRecord);
            await _dbContext.SaveChangesAsync();

            // TODO: gửi OTP qua email
            // _emailService.SendOtpEmail(existingUser.Email, otpRecord.OtpCode);

            return Ok(new
            {
                Message = "OTP sent to email",
                OTP = otpRecord.OtpCode // DEV ONLY - Remove in production!
            });
        }


        [HttpPost("VerifyOtp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequestDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u =>
                u.Email == request.Email
            );
            if (existingUser is null) return Unauthorized("Invalid OTP");

            var otpRecord = await _dbContext.OtpRecords
                .Where(otp => otp.UserId == existingUser.Id && !otp.IsUsed)
                .OrderByDescending(otp => otp.CreatedAtUtc)
                .FirstOrDefaultAsync();
            if (otpRecord is null || !_otpService.VerifyOtp(otpRecord, request.OtpCode))
                return Unauthorized("Invalid or expired OTP");

            return Ok("OTP verified successfully");
        }


        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequestDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u =>
                u.Email == request.Email
            );
            if (existingUser is null) return Unauthorized("Invalid request");

            // tìm OTP chưa sử dụng gần nhất
            var otpRecord = await _dbContext.OtpRecords
                .Where(otp => otp.UserId == existingUser.Id && !otp.IsUsed)
                .OrderByDescending(otp => otp.CreatedAtUtc)
                .FirstOrDefaultAsync();
            if (otpRecord is null || !_otpService.VerifyOtp(otpRecord, request.Otp))
                return Unauthorized("Invalid or expired OTP");

            // Hash password mới SAU KHI verify thành công
            existingUser.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword, 12);

            // đánh dấu OTP là đã sử dụng
            otpRecord.IsUsed = true;
            otpRecord.UsedAtUtc = DateTime.UtcNow;

            // revoke tất cả refresh token hiện có của user
            var allActiveTokens = await _dbContext.RefreshTokenRecords
                .Where(rft => rft.UserId == existingUser.Id && rft.RevokeAtUtc == null)
                .ToListAsync();

            foreach (var token in allActiveTokens)
                token.RevokeAtUtc = DateTime.UtcNow;

            // lưu thay đổi vào cơ sở dữ liệu
            await _dbContext.SaveChangesAsync();

            return Ok("Password reset successfully");
        }
    }
}