using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using MicroondasDigital.Models;
using MicroondasDigital.Models.Enums;
using MicroondasDigital.Exceptions;
using MicroondasDigital.Utils;

namespace MicroondasDigital.Controllers;

public class MicroondasController : Controller
{
    private AquecimentoModel _aquecimento;

    public MicroondasController()
    {
        _aquecimento = new AquecimentoModel(new HttpContextAccessor());
    }

    public IActionResult Index()
    {
        var model = _aquecimento.ResetarStatusMicroondasSeInvalido();        
        model.CustomPrograms = CustomProgramRepository.GetAll();

        return View(model);
    }

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
            
            return JsonValidationResult.CreateError(
            HtmlViewHelper.RenderPartialViewToString(this, "_CustomProgramModal", input));
        }
        if (existing.Any(p => (!isEdit || p.Id != input.Id)
                               && p.CaractereProgresso == input.CaractereProgresso))
        {
            BusinessException ex = new BusinessException("Este caractere de progressão já está cadastrado.");
            Logger.Log(ex);
            ModelState.AddModelError("CaractereProgresso", ex.Message);

            return JsonValidationResult.CreateError(
                HtmlViewHelper.RenderPartialViewToString(this, "_CustomProgramModal", input));
        }
        if (TipoAquecimentoConstants.CaracteresProgressoReservados.Contains(input.CaractereProgresso))
        {
            BusinessException ex = new BusinessException("Este caractere de progressão é reservado para os modos predefinidos.");
            Logger.Log(ex);
            ModelState.AddModelError("CaractereProgresso", ex.Message);

            return JsonValidationResult.CreateError(
                HtmlViewHelper.RenderPartialViewToString(this, "_CustomProgramModal", input));
        }

        return JsonValidationResult.CreateSuccess();
    }

    [HttpPost]
    public IActionResult Iniciar(MicroondasViewModel input)
    {
        if(!input.Validate())
        {
            foreach (var error in input.GetErrorLog())
            {
                ModelState.AddModelError(error.Key, error.Value);
            }            
            return View("Index", input); 
        }
        else
        {
            _aquecimento.IniciarAquecimento(input);
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    public IActionResult InicioRapido()
    {
        _aquecimento.IniciarAquecimentoRapido();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Pausar()
    {
        _aquecimento.PausarAquecimento();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Retomar()
    {
        _aquecimento.RetomarAquecimento();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Parar()
    {
        _aquecimento.PararAquecimento();
        return RedirectToAction(nameof(Index));
    }
#endregion

#region Receitas
    [HttpPost]
    public IActionResult AquecerPipoca()
    {
        _aquecimento.IniciarModoAquecimentoPredefinido(TipoAquecimento.Pipoca);
        return RedirectToAction(nameof(Index));
    }
    
    [HttpPost]
    public IActionResult AquecerLeite()
    {
        _aquecimento.IniciarModoAquecimentoPredefinido(TipoAquecimento.Leite);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult AquecerCarne()
    {
        _aquecimento.IniciarModoAquecimentoPredefinido(TipoAquecimento.Carne);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult AquecerFrango()
    {
        _aquecimento.IniciarModoAquecimentoPredefinido(TipoAquecimento.Frango);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult AquecerFeijao()
    {
        _aquecimento.IniciarModoAquecimentoPredefinido(TipoAquecimento.Feijao);
        return RedirectToAction(nameof(Index));
    }
#endregion

#region Programas Customizados
    [HttpGet]
    public IActionResult ListaProgramas()
    {
        var list = CustomProgramRepository.GetAll();
        return PartialView("_CustomProgramListModal", list);
    }
    
    [HttpPost]
    public IActionResult ExecuteCustomProgram(string programId)
    {
        var programas = CustomProgramRepository.GetAll();
        var programa = programas.FirstOrDefault(p => p.Id == programId);
        if (programa != null)
        {
            _aquecimento.IniciarAquecimento(
                TipoAquecimento.Customizado,
                programa.Tempo,
                programa.Potencia,
                programa.Instrucoes,
                programa.CaractereProgresso
            );
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult CriarProgramaCustomizado()
    {
        var model = new AquecimentoCustomizadoModel();
        return PartialView("_CustomProgramModal", model);
    }

    [HttpPost]
    [IgnoreAntiforgeryToken]
    public IActionResult CriarProgramaCustomizado(AquecimentoCustomizadoModel input)
    {
        if(!input.Validate())
        {
            foreach (var error in input.GetErrorLog())
            {
                ModelState.AddModelError(error.Key, error.Value);
            }
            
            return JsonValidationResult.CreateError(
                HtmlViewHelper.RenderPartialViewToString(this, "_CustomProgramModal", input));
        }

        JsonValidationResult validationResult = ValidarProgramaCustomizado(input);
        if(validationResult.IsValid)
        {
            CustomProgramRepository.Add(input);
        }
        
        return validationResult;
    }

    [HttpGet]
    public IActionResult EditarPrograma(string id)
    {
        var item = CustomProgramRepository.GetAll().FirstOrDefault(p => p.Id == id);
        if (item == null) return NotFound();
        
        return PartialView("_CustomProgramModal", item);
    }

    [HttpPost]
    [IgnoreAntiforgeryToken]
    public IActionResult EditarPrograma(AquecimentoCustomizadoModel input)
    {
        if (!input.Validate())
        {
            foreach (var error in input.GetErrorLog())
            {
                BusinessException ex = new BusinessException(error.Value);
                Logger.Log(ex);
                ModelState.AddModelError(error.Key, ex.Message);
            }
            return JsonValidationResult.CreateError(
                HtmlViewHelper.RenderPartialViewToString(this, "_CustomProgramModal", input));
        }

        JsonValidationResult validationResult = ValidarProgramaCustomizado(input);
        if(validationResult.IsValid)
        {
            CustomProgramRepository.Update(input);
        }
        
        return validationResult;
    }

    [HttpPost]
    [IgnoreAntiforgeryToken]
    public IActionResult DeletePrograma(string id)
    {
        CustomProgramRepository.Delete(id);
        return JsonValidationResult.CreateSuccess();
    }
#endregion

#region Atualização da Tela
    [HttpGet]
    public JsonResult StatusAtual()
    {
        return Json(_aquecimento.ObterJSONStatusMicroondas());
    }

    [HttpPost]
    [IgnoreAntiforgeryToken] 
    public IActionResult Tick()
    {
        return Json(_aquecimento.ExecutarProgressoAquecimento());
    }
#endregion

}
