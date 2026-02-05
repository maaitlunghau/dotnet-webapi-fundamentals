using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Shared.Domain;

namespace backend.Service;

public class TokenService
{
    private readonly IConfiguration _configuration;
    public TokenService(IConfiguration configuration) => _configuration = configuration;


    // create access token
    public (string accessToken, string jti) CreateAccessToken(User u)
    {
        // validate user input
        ArgumentNullException.ThrowIfNull(u);
        if (string.IsNullOrWhiteSpace(u.Email))
            throw new ArgumentException("User email is required.", nameof(u));
        if (string.IsNullOrWhiteSpace(u.Role))
            throw new ArgumentException("User role is required.", nameof(u));

        // create jti (unique identifier)
        var jti = Guid.NewGuid().ToString();

        // create claims
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, u.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, u.Email),
            new(ClaimTypes.Role, u.Role),
            new("Role", u.Role),
            new(JwtRegisteredClaimNames.Jti, jti)
        };

        // get secret key from appseting.json
        var keyText = _configuration["JWT:Key"]
            ?? throw new InvalidOperationException("JWT Key is not configured.");

        // convert format secret key (text) to byte array by SymmetricSecurityKey instance
        // because HMACSHA256 needs byte array key (not string) to work 
        // 
        // HMACSHA256 algorithmsL thuật toán ký của JWT
        // mục đích chính: tạo SIGNATURE để bảo vệ JWT token
        // ngoài ra:
        //      + tạo chữ ký cho JWT
        //      + đảm bảo JWT không bị giả mạo
        //      + xác thực JWT do server này tạo
        // 
        // Signagure là được mã hoá bằng thuật toán HMACSHA256
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyText));

        // create signing credentials
        var signingCredentials = new SigningCredentials(
            secretKey,
            SecurityAlgorithms.HmacSha256
        );

        // create lifetime of access token
        var minutes = int.Parse(_configuration["JWT:AccessTokenMinutes"] ?? "0");
        if (minutes <= 0)
            throw new InvalidOperationException("JWT AccessTokenMinutes is not configured properly.");

        // create JWT token
        var issuer = _configuration["JWT:Issuer"]
            ?? throw new InvalidOperationException("JWT Issuer is not configured.");
        var audience = _configuration["JWT:Audience"]
            ?? throw new InvalidOperationException("JWT Audience is not configured.");

        var jwtToken = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(minutes),
            signingCredentials: signingCredentials
        );

        // return access token and jti
        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);
        return (accessToken, jti);
    }


    // create refresh token
    public RefreshTokenRecord CreateRefreshToken(Guid userId, string accessTokenJti)
    {
        // validate input
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));
        if (string.IsNullOrWhiteSpace(accessTokenJti))
            throw new ArgumentException("Access token JTI is required.", nameof(accessTokenJti));

        // create lifetime of refresh token
        var days = int.Parse(_configuration["JWT:RefreshTokenDays"] ?? "0");
        if (days <= 0)
            throw new InvalidOperationException("JWT RefreshTokenDays is not configured properly.");

        // create refresh token
        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        // return refresh token record
        return new RefreshTokenRecord
        {
            // link to user
            UserId = userId,

            // create refresh token
            RefreshToken = refreshToken,

            // gán mã định danh của access token (Jti)
            AccessTokenJti = accessTokenJti,

            // set expire time
            ExpireAtUtc = DateTime.UtcNow.AddDays(days)
        };
    }
}