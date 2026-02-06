using Shared.Domain;

namespace backend.Services;

public class OtpService
{
    private readonly IConfiguration _configuration;
    public OtpService(IConfiguration configuration)
        => _configuration = configuration;

    public OtpRecord CreateOtp(Guid userId)
    {
        var otp = Random.Shared.Next(100000, 999999).ToString();
        var expiryMinutes = int.Parse(_configuration["OTP:ExpiryMinutes"] ?? "5");

        return new OtpRecord
        {
            UserId = userId,
            OtpCode = otp,
            ExpireAtUtc = DateTime.UtcNow.AddMinutes(expiryMinutes)
        };
    }

    public bool VerifyOtp(OtpRecord otpRecord, string inputOtp)
    {
        return otpRecord.IsActive && otpRecord.OtpCode == inputOtp;
    }
}