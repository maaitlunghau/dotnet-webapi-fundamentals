using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Domain;

public class RefreshTokenRecord
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid UserId { get; set; }
    // nhận biết người dùng nào sở hữu token này

    public string RefreshToken { get; set; }
    // giá trị của refresh token

    public string AccessTokenJti { get; set; }
    // Jti (JWT Token Identifier) = JWT ID: là một claim chuẩn trong JWT
    // mã định dang DUY NHẤT cho mỗi JWT token

    // giúp nhận biết và phân biệt các token với nhau
    // nhận biết được refresh token này được tạo cho access token nào
    // => trong trường hợp cần thu hồi hoặc quản lý token

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    // thời điểm tạo refresh token

    public DateTime ExpireAtUtc { get; set; }
    // thời điểm hết hạn của refresh token

    public string? ReplacedByRefreshToken { get; set; }
    // token mới thay thế token hiện tại (nếu hết hạn hoặc bị thu hồi)
    // null: nếu token chưa bị thay thế

    public DateTime? RevokeAtUtc { get; set; }
    // thời điểm token bị thu hồi
    // null: nếu token vẫn còn hiệu lực

    [NotMapped]
    public bool IsExpired => DateTime.UtcNow >= ExpireAtUtc;
    // kiểm tra token đã hết hạn chưa

    [NotMapped]
    public bool IsActive => RevokeAtUtc == null && !IsExpired;
    // kiểm tra token còn hiệu lực hay không

    // navigation property
    public User? User { get; set; }
}