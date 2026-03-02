using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using MicroondasDigital.Models;
using MicroondasDigital.Models.Enums;
using System.Runtime.InteropServices.Marshalling;

namespace MicroondasDigital.Controllers.Api;

[ApiController]
[Route("api/v1/[controller]")]
public class MicroondasApiController : ControllerBase
{
    private readonly IConfiguration _config;
    
    // Instanciamos o model passando "null" porque na API nós gerenciaremos a session diretamente
    // ou se você adaptou o AquecimentoModel para receber o Contexto, adapte aqui.
    public MicroondasApiController(IConfiguration config)
    {
        _config = config;
    }

    // ==========================================
    // AUTHENTICATION ENDPOINTS
    // ==========================================
    [HttpPost("auth/login")]
    [AllowAnonymous]
    public IActionResult Login([FromBody] LoginViewModel model)
    {
        // Regra do desafio: SHA256. 
        // Hash de "123456" é 8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92
        string hashEsperado = "8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92";

        if (model.Username != "admin" || Sha256Hash(model.Password) != hashEsperado)
        {
            return Unauthorized(new { success = false, message = "Credenciais inválidas" });
        }

        var token = GenerateJwtToken(model.Username);
        
        return Ok(new AuthResponse 
        { 
            Token = token,
            Expires = DateTime.UtcNow.AddMinutes(60),
            IsAuthenticated = true
        });
    }

    [HttpGet("auth/check")]
    [Authorize]
    public IActionResult CheckAuth()
    {
        return Ok(new { success = true, message = "Token válido", user = User.Identity?.Name });
    }

    // ==========================================
    // STATUS ENDPOINTS
    // ==========================================
    [HttpGet("status")]
    [Authorize]
    public IActionResult GetStatus()
    {
        var model = SessionHelper.GetStatusMicroondas(HttpContext);
        
        return Ok(new 
        {
            tempoFormatado = $"{model.TempoAquecimento / 60:D2}:{model.TempoAquecimento % 60:D2}",
            display = model.Display,
            status = model.Status,
            isRunning = model.IsRunning(),
            isPaused = model.IsPaused()
        });
    }

    [HttpPost("tick")]
    [Authorize]
    public IActionResult Tick()
    {
        var model = SessionHelper.GetStatusMicroondas(HttpContext);
        
        if (model.IsRunning() && !model.IsPaused() && model.TempoAquecimento > 0)
        {
            model.TempoAquecimento--;
            model.Display += new string(model.CaractereProgresso, model.Potencia);
            
            if (model.TempoAquecimento <= 0)
            {
                model.StatusEnum = StatusAquecimento.Parado;
                model.Display += " Aquecimento concluído!";
            }
        }
        
        SessionHelper.SetStatusMicroondas(HttpContext, model);
        
        return Ok(new 
        {
            tempoFormatado = $"{model.TempoAquecimento / 60:D2}:{model.TempoAquecimento % 60:D2}",
            display = model.Display,
            status = model.Status,
            isRunning = model.IsRunning(),
            isPaused = model.IsPaused()
        });
    }

    // ==========================================
    // CONTROL ENDPOINTS
    // ==========================================
    [HttpPost("iniciar")]
    [Authorize]
    public IActionResult Iniciar([FromBody] MicroondasViewModel input)
    {
        var model = SessionHelper.GetStatusMicroondas(HttpContext);

        // Se está pausado, retoma
        if (model.StatusEnum == StatusAquecimento.Pausado)
        {
            model.StatusEnum = StatusAquecimento.Aquecendo;
            SessionHelper.SetStatusMicroondas(HttpContext, model);
            return Ok(new { success = true, message = "Retomado" });
        }

        // Se é novo
        model.TempoAquecimento = input.TempoAquecimento;
        model.Potencia = input.Potencia == 0 ? Constants.PotenciaPadrao : input.Potencia;
        model.StatusEnum = StatusAquecimento.Parado;

        if (!model.Validate())
        {
            return BadRequest(new { success = false, errors = model.GetErrorLog() });
        }

        model.StatusEnum = StatusAquecimento.Aquecendo;
        model.Display = "";
        
        SessionHelper.SetStatusMicroondas(HttpContext, model);
        return Ok(new { success = true, message = "Iniciado" });
    }

    [HttpPost("inicio-rapido")]
    [Authorize]
    public IActionResult InicioRapido()
    {
        var model = SessionHelper.GetStatusMicroondas(HttpContext);

        if (model.StatusEnum == StatusAquecimento.Parado || model.StatusEnum == StatusAquecimento.Parado)
        {
            model.TempoAquecimento = Constants.TempoAquecimentoInicioRapido;
            model.Potencia = Constants.PotenciaPadrao;
            model.StatusEnum = StatusAquecimento.Aquecendo;
            model.Display = "";
        }
        else
        {
            model.TempoAquecimento += Constants.TempoAquecimentoInicioRapido;
        }

        SessionHelper.SetStatusMicroondas(HttpContext, model);
        return Ok(new { success = true, message = "Início Rápido acionado" });
    }

    [HttpPost("pausar-parar")]
    [Authorize]
    public IActionResult PausarParar()
    {
        var model = SessionHelper.GetStatusMicroondas(HttpContext);

        if (model.StatusEnum == StatusAquecimento.Aquecendo)
        {
            model.StatusEnum = StatusAquecimento.Pausado;
            SessionHelper.SetStatusMicroondas(HttpContext, model);
            return Ok(new { success = true, message = "Pausado" });
        }

        // Se pausado ou parado, limpa (cancela)
        SessionHelper.ClearStatusMicroondas(HttpContext);
        return Ok(new { success = true, message = "Cancelado/Limpado" });
    }

    // ==========================================
    // CUSTOM PROGRAMS (OPCIONAL API EXPOSE)
    // ==========================================
    [HttpGet("programas")]
    [Authorize]
    public IActionResult GetProgramas()
    {
        var list = CustomProgramRepository.GetAll();
        return Ok(list);
    }

    // ==========================================
    // HELPERS
    // ==========================================
    private static string Sha256Hash(string input)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    private string GenerateJwtToken(string username)
    {
        // Se colocar a config no appsettings.json use _config["Jwt:Key"], senão use hardcoded para o teste:
        var jwtKey = _config["Jwt:Key"] ?? "MicroondasSuperSecretKey1234567890+";
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"] ?? "MicroondasAPI",
            audience: _config["Jwt:Audience"] ?? "MicroondasClient",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(60),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
