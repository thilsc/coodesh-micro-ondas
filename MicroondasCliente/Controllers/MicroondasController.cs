using Microsoft.AspNetCore.Mvc;
using MicroondasCliente.Models;
using MicroondasCliente.Utils;

namespace MicroondasCliente.Controllers;

public class MicroondasController : Controller
{
    private MicroondasViewModel CriarModeloDeErro(string mensagem)
    {
        return new MicroondasViewModel
        {
            MensagemDeErro = mensagem,
            StatusAtual = MicroondasCliente.Models.Enums.StatusAquecimento.Parado,
            PotenciaAtual = 10
        };
    }

    #region Funções básicas
    [HttpPost]
    public async Task<IActionResult> Iniciar([FromForm] int tempoAquecimento, [FromForm] int potencia)
    {
        try
        {
            var payload = new { TempoAquecimento = tempoAquecimento, Potencia = potencia };
            var response = await ApiResponseHelper.ObterRetornoEndpointApiPost("iniciar", payload);

            if (!response.IsSuccessStatusCode)
                TempData["MensagemErro"] = "Erro ao iniciar o micro-ondas.";
        }
        catch (Exception ex)
        {
            TempData["MensagemErro"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> InicioRapido()
    {
        try
        {
            var response = await ApiResponseHelper.ObterRetornoEndpointApiPost("inicio-rapido");

            if (!response.IsSuccessStatusCode)
                TempData["MensagemErro"] = "Erro ao iniciar rápido.";
        }
        catch (Exception ex)
        {
            TempData["MensagemErro"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Pausar()
    {
        try
        {
            var response = await ApiResponseHelper.ObterRetornoEndpointApiPost("pausar");

            if (!response.IsSuccessStatusCode)
                TempData["MensagemErro"] = "Erro ao pausar.";
        }
        catch (Exception ex)
        {
            TempData["MensagemErro"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Retomar()
    {
        try
        {
            var response = await ApiResponseHelper.ObterRetornoEndpointApiPost("retomar");

            if (!response.IsSuccessStatusCode)
                TempData["MensagemErro"] = "Erro ao retomar.";
        }
        catch (Exception ex)
        {
            TempData["MensagemErro"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Parar()
    {
        try
        {
            var response = await ApiResponseHelper.ObterRetornoEndpointApiPost("parar");

            if (!response.IsSuccessStatusCode)
                TempData["MensagemErro"] = "Erro ao parar.";
        }
        catch (Exception ex)
        {
            TempData["MensagemErro"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
    #endregion

    #region Receitas
    [HttpPost]
    public async Task<IActionResult> AquecerPipoca()
    {
        try
        {
            var response = await ApiResponseHelper.ObterRetornoEndpointApiPost("aquecer-pipoca");

            if (!response.IsSuccessStatusCode)
                TempData["MensagemErro"] = "Erro ao aquecer pipoca.";
        }
        catch (Exception ex)
        {
            TempData["MensagemErro"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> AquecerLeite()
    {
        try
        {
            var response = await ApiResponseHelper.ObterRetornoEndpointApiPost("aquecer-leite");

            if (!response.IsSuccessStatusCode)
                TempData["MensagemErro"] = "Erro ao aquecer leite.";
        }
        catch (Exception ex)
        {
            TempData["MensagemErro"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> AquecerCarne()
    {
        try
        {
            var response = await ApiResponseHelper.ObterRetornoEndpointApiPost("aquecer-carne");

            if (!response.IsSuccessStatusCode)
                TempData["MensagemErro"] = "Erro ao aquecer carne.";
        }
        catch (Exception ex)
        {
            TempData["MensagemErro"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> AquecerFrango()
    {
        try
        {
            var response = await ApiResponseHelper.ObterRetornoEndpointApiPost("aquecer-frango");

            if (!response.IsSuccessStatusCode)
                TempData["MensagemErro"] = "Erro ao aquecer frango.";
        }
        catch (Exception ex)
        {
            TempData["MensagemErro"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> AquecerFeijao()
    {
        try
        {
            var response = await ApiResponseHelper.ObterRetornoEndpointApiPost("aquecer-feijao");

            if (!response.IsSuccessStatusCode)
                TempData["MensagemErro"] = "Erro ao aquecer feijão.";
        }
        catch (Exception ex)
        {
            TempData["MensagemErro"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
    #endregion

    #region Programas Customizados
    private async Task<List<AquecimentoCustomizadoModel>> GetProgramas()
    {
        try
        {
            return await ApiResponseHelper.ObterRetornoEndpointApiGet<List<AquecimentoCustomizadoModel>>("programas") ?? new();
        }
        catch
        {
            return new();
        }
    }

    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> CriarProgramaCustomizado(AquecimentoCustomizadoModel input)
    {
        if (!input.Validate())
        {
            foreach (var error in input.GetErrorLog())
                ModelState.AddModelError(error.Key, error.Value);
            
            return JsonValidationResult.CreateError(
                HtmlViewHelper.RenderPartialViewToString(this, "_CustomProgramModal", input));
        }

        try
        {
            var response = await ApiResponseHelper.ObterRetornoEndpointApiPost("criar-programa", input);

            if (!response.IsSuccessStatusCode)
                return JsonValidationResult.CreateError("Erro ao criar programa");

            return JsonValidationResult.CreateSuccess();
        }
        catch (Exception ex)
        {
            return JsonValidationResult.CreateError(ex.Message);
        }
    }

    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> EditarPrograma(AquecimentoCustomizadoModel input)
    {
        if (!input.Validate())
        {
            foreach (var error in input.GetErrorLog())
                ModelState.AddModelError(error.Key, error.Value);
            
            return JsonValidationResult.CreateError(
                HtmlViewHelper.RenderPartialViewToString(this, "_CustomProgramModal", input));
        }

        try
        {
            var response = await ApiResponseHelper.ObterRetornoEndpointApiPost("editar-programa", input);

            if (!response.IsSuccessStatusCode)
                return JsonValidationResult.CreateError("Erro ao editar programa");

            return JsonValidationResult.CreateSuccess();
        }
        catch (Exception ex)
        {
            return JsonValidationResult.CreateError(ex.Message);
        }
    }

    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> DeletePrograma(string id)
    {
        try
        {
            var response = await ApiResponseHelper.ObterRetornoEndpointApiPost($"delete-programa?id={id}");

            if (!response.IsSuccessStatusCode)
                return JsonValidationResult.CreateError("Erro ao deletar programa");

            return JsonValidationResult.CreateSuccess();
        }
        catch (Exception ex)
        {
            return JsonValidationResult.CreateError(ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> ExecuteCustomProgram(string programId)
    {
        try
        {
            var response = await ApiResponseHelper.ObterRetornoEndpointApiPost($"executar-programa?id={programId}");

            if (!response.IsSuccessStatusCode)
                TempData["MensagemErro"] = "Erro ao executar programa.";
        }
        catch (Exception ex)
        {
            TempData["MensagemErro"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
    #endregion

    #region Atualização da Tela
    private async Task<MicroondasViewModel> ObterStatusDaApi()
    {
        try
        {
            var model = await ApiResponseHelper.ObterRetornoEndpointApiGet<MicroondasViewModel>("status");
            return model ?? new MicroondasViewModel();
        }
        catch (HttpRequestException)
        {
            return CriarModeloDeErro("Sessão expirada. Faça login novamente.");
        }
        catch (Exception ex)
        {
            return CriarModeloDeErro($"Erro da API: {ex.Message}");
        }
    }

    [HttpGet]
    public async Task<IActionResult> StatusAtual()
    {
        try
        {
            var data = await ApiResponseHelper.ObterRetornoEndpointApiGet<dynamic>("status");
            return Json(data);
        }
        catch (Exception ex)
        {
            return Json(new { error = ex.Message });
        }
    }

    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<JsonResult> Tick()
    {
        try
        {
            var data = await ApiResponseHelper.ObterRetornoEndpointApiPost<dynamic>("tick");
            return Json(data);
        }
        catch (Exception ex)
        {
            return Json(new { error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var token = HttpContext.Session.GetString("JWTToken");

        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Account");

        var model = await ObterStatusDaApi();
        
        if ((model.MensagemDeErro != null) && model.MensagemDeErro.Contains("login"))
        {
            TempData["MensagemErro"] = model.MensagemDeErro;
            return RedirectToAction("Logout", "Account");
        }

        model.CustomPrograms = (await GetProgramas()) ?? new List<AquecimentoCustomizadoModel>();

        return View(model);
    }
    #endregion
}
