using _05_authentication.Models;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using Microsoft.IdentityModel.Tokens;

namespace _05_authentication.Services;

public class TokenService
{
    protected readonly IConfiguration _configuration;
    public TokenService(IConfiguration configuration) =>
        _configuration = configuration;

    // create access token
    public (string accessToken, string jti) CreateAccessToken(Account acc)
    {
        var jti = Guid.NewGuid().ToString();

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, acc.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, acc.Email),
            new(ClaimTypes.Role, acc.Role.ToString()),
            new("Role", acc.Role),
            new(JwtRegisteredClaimNames.Jti, jti)
        };

        // get secret key from appsettings.json
        var keyText = _configuration["Jwt:Key"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyText!));

        // sign TOKEN and KEY
        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256 // thuật toán để xử lí ?
        );

        // tạo thời gian sống của accessToken 
        var minutes = int.Parse(_configuration["Jwt:AccessTokenMinutes"]!);

        // Tạo JWT token với thông tin cấu hình
        var jwt = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(minutes),
            signingCredentials: credentials
        );

        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);
        return (accessToken, jti);
    }

    // create refresh token
    public RefreshTokenRecord CreateRefreshtoken(Guid AccountId, string accessTokenJti)
    {
        // tạo thời gian sống refreshToken
        var days = int.Parse(_configuration["Jwt:RefreshTokenDays"]!);

        return new RefreshTokenRecord
        {
            // chủ sở hữu Token
            AccountId = AccountId,

            // mã định danh Token (Jti)
            AccessTokenJti = accessTokenJti,
            RefreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            ExpireAtUtc = DateTime.UtcNow.AddDays(days)
        };
    }
}