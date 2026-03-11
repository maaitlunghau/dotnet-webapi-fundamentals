using System.ComponentModel.DataAnnotations;

namespace server.Models;

public class Account
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    [RegularExpression("^(ADMIN|USER)$", ErrorMessage = "Role must be ADMIN or USER")]
    public string Role { get; set; } = string.Empty;

    public bool Status { get; set; }
}
