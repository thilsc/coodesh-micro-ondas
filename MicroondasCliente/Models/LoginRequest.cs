using System.ComponentModel.DataAnnotations;

namespace MicroondasCliente.Models;

public class LoginRequest
{
    [Required(ErrorMessage = "O usuário é obrigatório")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}
