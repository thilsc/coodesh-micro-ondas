namespace MicroondasCliente.Models;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public bool IsAuthenticated { get; set; }
}
