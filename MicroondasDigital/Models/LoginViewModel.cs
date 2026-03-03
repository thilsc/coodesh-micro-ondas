using System.ComponentModel.DataAnnotations;

namespace MicroondasDigital.Models;

public class LoginViewModel
{
    [Required]
    public string Username { get; set; } = "admin";

    [Required]
    public string Password { get; set; } = "123456";
}

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expires { get; set; }
    public bool IsAuthenticated { get; set; }
}
