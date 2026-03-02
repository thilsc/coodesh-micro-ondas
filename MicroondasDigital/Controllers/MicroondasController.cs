using Microsoft.AspNetCore.Mvc;
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
            
            return JsonValidationResult.CreateError(RenderPartialViewToString("_CustomProgramModal", input));
        }
        if (existing.Any(p => (!isEdit || p.Id != input.Id)
                               && p.CaractereProgresso == input.CaractereProgresso))
        {
            BusinessException ex = new BusinessException("Este caractere de progressão já está cadastrado.");
            Logger.Log(ex);
            ModelState.AddModelError("CaractereProgresso", ex.Message);

            return JsonValidationResult.CreateError(RenderPartialViewToString("_CustomProgramModal", input));
        }
        if (TipoAquecimentoConstants.CaracteresProgressoReservados.Contains(input.CaractereProgresso))
        {
            BusinessException ex = new BusinessException("Este caractere de progressão é reservado para os modos predefinidos.");
            Logger.Log(ex);
            ModelState.AddModelError("CaractereProgresso", ex.Message);

            return JsonValidationResult.CreateError(RenderPartialViewToString("_CustomProgramModal", input));
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
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    public IActionResult InicioRapido()
    {
        _aquecimento.IniciarAquecimentoRapido();
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Pausar()
    {
        _aquecimento.PausarAquecimento();
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Retomar()
    {
        _aquecimento.RetomarAquecimento();               
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Parar()
    {
        _aquecimento.PararAquecimento();
        return RedirectToAction("Index");
    }    
#endregion

#region Receitas
    [HttpPost]
    public IActionResult AquecerPipoca()
    {
        _aquecimento.IniciarModoAquecimentoPredefinido(TipoAquecimento.Pipoca);
        return RedirectToAction("Index");
    }
    
    [HttpPost]
    public IActionResult AquecerLeite()
    {
        _aquecimento.IniciarModoAquecimentoPredefinido(TipoAquecimento.Leite);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult AquecerCarne()
    {
        _aquecimento.IniciarModoAquecimentoPredefinido(TipoAquecimento.Carne);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult AquecerFrango()
    {
        _aquecimento.IniciarModoAquecimentoPredefinido(TipoAquecimento.Frango);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult AquecerFeijao()
    {
        _aquecimento.IniciarModoAquecimentoPredefinido(TipoAquecimento.Feijao);
        return RedirectToAction("Index");
    }
#endregion

#region Programas Customizados
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
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult ListaProgramas()
    {
        var list = CustomProgramRepository.GetAll();
        return PartialView("_CustomProgramListModal", list);
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
            
            return JsonValidationResult.CreateError(RenderPartialViewToString("_CustomProgramModal", input));
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

    // POST edição
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
            return JsonValidationResult.CreateError(RenderPartialViewToString("_CustomProgramModal", input));
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

    private string RenderPartialViewToString(string viewName, object model)
    {
        ViewData.Model = model;
        using var writer = new System.IO.StringWriter();
        var viewResult = HttpContext.RequestServices
            .GetService<Microsoft.AspNetCore.Mvc.ViewEngines.ICompositeViewEngine>()
            .FindView(ControllerContext, viewName, false);

        if (viewResult.View == null)
            throw new ArgumentNullException($"View '{viewName}' não encontrada.");

        var viewContext = new Microsoft.AspNetCore.Mvc.Rendering.ViewContext(
            ControllerContext,
            viewResult.View,
            ViewData,
            TempData,
            writer,
            new Microsoft.AspNetCore.Mvc.ViewFeatures.HtmlHelperOptions()
        );
        viewResult.View.RenderAsync(viewContext).GetAwaiter().GetResult();
        return writer.GetStringBuilder().ToString();
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
