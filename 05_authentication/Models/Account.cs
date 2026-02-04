using System.ComponentModel.DataAnnotations;

namespace _05_authentication.Models;

public class Account
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required(ErrorMessage = "Email tài khoản không được bỏ trống!")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu tài khoản không được bỏ trống!")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vai trò tài khoản không được bỏ trống!")]
    public string Role { get; set; } = "user";

    // navigation property
    public List<RefreshTokenRecord>? RefreshTokens { get; set; }
}