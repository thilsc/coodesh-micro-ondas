using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MicroondasDigital.Models;
using MicroondasDigital.Models.Enums;
using MicroondasDigital.Utils;
using MicroondasDigital.Exceptions;

namespace MicroondasDigital.Controllers.Api;

[ApiController]
[Route("api/v1/[controller]")]
public class MicroondasApiController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly string _userName = "admin";
    private readonly string _password = "123456";

    private static void ConfigurarAquecimentoPredefinido(MicroondasViewModel model, TipoAquecimento modoAquecimento)
    {
        model.TempoAquecimento   = TipoAquecimentoConstants.GetTempoAquecimento(modoAquecimento);
        model.Potencia           = TipoAquecimentoConstants.GetPotencia(modoAquecimento);
        model.Instrucoes         = TipoAquecimentoConstants.GetInstrucoes(modoAquecimento);
        model.CaractereProgresso = TipoAquecimentoConstants.GetProgressChar(modoAquecimento);
        model.StatusEnum         = StatusAquecimento.Aquecendo;
        model.Display            = "";
    }

    public MicroondasApiController(IConfiguration config)
    {
        _configuration = config;
    }

#region Métodos de autenticação
    [HttpPost("auth/login")]
    [AllowAnonymous]
    public IActionResult Login([FromBody] LoginViewModel model)
    {
        if ( !model.Username.Equals(_userName) || 
            (!HashHelper.CalcularSha256(model.Password).Equals(HashHelper.CalcularSha256(_password))) )
        {
            return Unauthorized(new { IsAuthenticated = false, message = "Credenciais inválidas" });
        }

        //var token = GenerateJwtToken(model.Username);
        var issuer    = _configuration["Jwt:Issuer"];
        var audience  = _configuration["Jwt:Audience"];
        var secretKey = _configuration["Jwt:SecretKey"];

        if (string.IsNullOrEmpty(secretKey))
        {
            return StatusCode(500, "SecretKey do JWT não configurada no servidor.");
        }

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, model.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        DateTime DefaultSessionTime = DateTime.UtcNow.AddMinutes(60);  //máx 1 hora na sesssão

        var tokenDescriptor = new SecurityTokenDescriptor 
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DefaultSessionTime,
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = credentials
        };

        var tokenHandler  = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString   = tokenHandler.WriteToken(securityToken);

        return Ok(new AuthResponse 
        { 
            Token = tokenString,
            Expires = tokenDescriptor.Expires ?? DefaultSessionTime, 
            IsAuthenticated = true
        });
    }
    
    [HttpGet("auth/check")]
    [Authorize]
    public IActionResult CheckAuth()
    {
        return Ok(new { success = true, message = "Token válido", user = User.Identity?.Name });
    }
#endregion

#region Funções básicas
    private JsonValidationResult ValidarProgramaCustomizado(AquecimentoCustomizadoModel input)
    {
        var existing = CustomProgramRepository.GetAll();
        bool isEdit = !string.IsNullOrWhiteSpace(input.Id);

        if (existing.Any(p => (!isEdit || p.Id != input.Id)
                               && p.Nome.Equals(input.Nome, StringComparison.OrdinalIgnoreCase)))
        {
            BusinessException ex = new BusinessException("Já existe um programa com esse nome.");
            Logger.Log(ex);
            ModelState.AddModelError("Nome", ex.Message);
            
            return JsonValidationResult.CreateError(ex.Message);
        }
        if (existing.Any(p => (!isEdit || p.Id != input.Id)
                               && p.CaractereProgresso == input.CaractereProgresso))
        {
            BusinessException ex = new BusinessException("Este caractere de progressão já está cadastrado.");
            Logger.Log(ex);
            ModelState.AddModelError("CaractereProgresso", ex.Message);

            return JsonValidationResult.CreateError(ex.Message);
        }
        if (TipoAquecimentoConstants.CaracteresProgressoReservados.Contains(input.CaractereProgresso))
        {
            BusinessException ex = new BusinessException("Este caractere de progressão é reservado para os modos predefinidos.");
            Logger.Log(ex);
            ModelState.AddModelError("CaractereProgresso", ex.Message);

            return JsonValidationResult.CreateError(ex.Message);
        }

        return JsonValidationResult.CreateSuccess();
    }

    [HttpPost("iniciar")]
    [Authorize]
    public IActionResult Iniciar([FromBody] MicroondasViewModel input)
    {
        var model = SessionHelper.GetStatusMicroondas(HttpContext);

        if (model.StatusEnum == StatusAquecimento.Pausado)
        {
            model.StatusEnum = StatusAquecimento.Aquecendo;
            SessionHelper.SetStatusMicroondas(HttpContext, model);
            return Ok(new { success = true, message = "Retomado" });
        }

        if (input.TempoAquecimento < 1)
        {
            return BadRequest(new { success = false, errors = new { TempoAquecimento = "O Tempo não pode ser menor que 1." } });
        }

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

    [HttpPost("pausar")]
    [Authorize]
    public IActionResult Pausar()
    {
        var model = SessionHelper.GetStatusMicroondas(HttpContext);

        if (model.StatusEnum == StatusAquecimento.Aquecendo)
        {
            model.StatusEnum = StatusAquecimento.Pausado;
            SessionHelper.SetStatusMicroondas(HttpContext, model);
            return Ok(new { success = true, message = "Aquecimento pausado." });
        }
        return BadRequest(new { success = false, message = "Não é possível pausar neste estado." });
    }

    [HttpPost("retomar")]
    [Authorize]
    public IActionResult Retomar()
    {
        var model = SessionHelper.GetStatusMicroondas(HttpContext);

        if (model.StatusEnum == StatusAquecimento.Pausado)
        {
            model.StatusEnum = StatusAquecimento.Aquecendo;
            SessionHelper.SetStatusMicroondas(HttpContext, model);
            return Ok(new { success = true, message = "Aquecimento retomado." });
        }
        return BadRequest(new { success = false, message = "Não é possível retomar neste estado." });
    }

    [HttpPost("parar")]
    [Authorize]
    public IActionResult Parar()
    {
        SessionHelper.ClearStatusMicroondas(HttpContext);
        return Ok(new { success = true, message = "Parado." });
    }        
#endregion

#region Receitas
    [HttpPost("aquecer-pipoca")]
    [Authorize]
    public IActionResult AquecerPipoca()
    {
        var model = SessionHelper.GetStatusMicroondas(HttpContext);
        ConfigurarAquecimentoPredefinido(model, TipoAquecimento.Pipoca);
        SessionHelper.SetStatusMicroondas(HttpContext, model);

        return Ok(new { success = true, message = "Pipoca aquecendo" });
    }
    
    [HttpPost("aquecer-leite")]
    [Authorize]
    public IActionResult AquecerLeite()
    {
        var model = SessionHelper.GetStatusMicroondas(HttpContext);
        ConfigurarAquecimentoPredefinido(model, TipoAquecimento.Leite);
        SessionHelper.SetStatusMicroondas(HttpContext, model);

        return Ok(new { success = true, message = "Leite aquecendo" });
    }

    [HttpPost("aquecer-carne")]
    [Authorize]
    public IActionResult AquecerCarne()
    {
        var model = SessionHelper.GetStatusMicroondas(HttpContext);
        ConfigurarAquecimentoPredefinido(model, TipoAquecimento.Carne);
        SessionHelper.SetStatusMicroondas(HttpContext, model);

        return Ok(new { success = true, message = "Carne aquecendo" });
    }

    [HttpPost("aquecer-frango")]
    [Authorize]
    public IActionResult AquecerFrango()
    {
        var model = SessionHelper.GetStatusMicroondas(HttpContext);
        ConfigurarAquecimentoPredefinido(model, TipoAquecimento.Frango);
        SessionHelper.SetStatusMicroondas(HttpContext, model);

        return Ok(new { success = true, message = "Frango aquecendo" });
    }

    [HttpPost("aquecer-feijao")]
    [Authorize]
    public IActionResult AquecerFeijao()
    {
        var model = SessionHelper.GetStatusMicroondas(HttpContext);
        ConfigurarAquecimentoPredefinido(model, TipoAquecimento.Feijao);
        SessionHelper.SetStatusMicroondas(HttpContext, model);

        return Ok(new { success = true, message = "Feijão aquecendo" });
    }
#endregion

#region Programas Customizados
    [HttpGet("programas")]
    [Authorize]
    public IActionResult ListaProgramas()
    {
        var list = CustomProgramRepository.GetAll();
        return Ok(list);
    }

    [HttpPost("executar-programa")]
    [Authorize]
    [IgnoreAntiforgeryToken]
    public IActionResult ExecuteCustomProgram(string id)
    {
        var programas = CustomProgramRepository.GetAll();
        var programa = programas.FirstOrDefault(p => p.Id == id);
        if (programa == null)        {
            return NotFound(new { success = false, message = "Programa não encontrado." });
        }
        var model = SessionHelper.GetStatusMicroondas(HttpContext);
        model.TempoAquecimento = programa.Tempo;
        model.Potencia = programa.Potencia;
        model.StatusEnum = StatusAquecimento.Aquecendo;
        model.Display = "";

        SessionHelper.SetStatusMicroondas(HttpContext, model);
        return Ok(new { success = true, message = "Programa em execução.", programa });
    }

    [HttpPost("criar-programa")]
    [Authorize]
    [IgnoreAntiforgeryToken]
    public IActionResult CriarProgramaCustomizado(AquecimentoCustomizadoModel input)
    {
        if (string.IsNullOrEmpty(input.Nome))
        {
            return BadRequest(new { success = false, errors = new { Nome = "O nome do programa é obrigatório." } });
        }
        if (input.Tempo < 1)
        {
            return BadRequest(new { success = false, errors = new { Tempo = "O tempo deve ser maior que 0." } });
        }
        if (input.Potencia < 1 || input.Potencia > 10)
        {
            return BadRequest(new { success = false, errors = new { Potencia = "A potência deve ser entre 1 e 10." } });
        }

        var programa = new AquecimentoCustomizadoModel
        {
            Id = Guid.NewGuid().ToString(),
            Nome = input.Nome,
            Tempo = input.Tempo,
            Potencia = input.Potencia,
            Instrucoes = input.Instrucoes,
            CaractereProgresso = input.CaractereProgresso
        };

        CustomProgramRepository.Add(programa);
        return Ok(new { success = true, message = "Programa criado com sucesso.", programa });
    }

    [HttpPost("editar-programa")]
    [Authorize]
    [IgnoreAntiforgeryToken]
    public IActionResult EditarPrograma(AquecimentoCustomizadoModel input)
    {
        if (!input.Validate())
        {
            var errors = input.GetErrorLog();
            return BadRequest(new { success = false, errors = errors });
        }
        
        JsonValidationResult validationResult = ValidarProgramaCustomizado(input);
        if(!validationResult.IsValid)
        {
            return BadRequest(new { success = false, message = "Não foi possível editar o programa." });
        }

        CustomProgramRepository.Update(input);
        return Ok(new { success = true, message = "Programa atualizado com sucesso.", programa = input });
    }

    [HttpPost("delete-programa")]
    [Authorize]
    [IgnoreAntiforgeryToken]
    public IActionResult DeletePrograma(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return BadRequest(new { success = false, message = "ID do programa é obrigatório." });
        }

        CustomProgramRepository.Delete(id);
        return Ok(new { success = true, message = "Programa removido com sucesso." });
    }
#endregion

#region Atualização da Tela
    [HttpGet("status")]
    [Authorize]
    public IActionResult StatusAtual()
    {
        var model = SessionHelper.GetStatusMicroondas(HttpContext);
        
        return Ok(new 
        {
            tempoFormatado = $"{model.TempoAquecimento / 60:D2}:{model.TempoAquecimento % 60:D2}",
            display = model.Display,
            status = model.Status,
            instrucoes = model.Instrucoes,
            isRunning = model.IsRunning(),
            isPaused = model.IsPaused(),
            ismodoAquecimentoPadrao = model.StatusEnum == StatusAquecimento.Aquecendo
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
#endregion
}
