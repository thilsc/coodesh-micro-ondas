using Microsoft.AspNetCore.Mvc;
using MicroondasDigital.Models;
using MicroondasDigital.Models.Enums;

namespace MicroondasDigital.Controllers;

public class MicroondasController : Controller
{
    private AquecimentoModel _aquecimento;

    public MicroondasController()
    {
        _aquecimento = new AquecimentoModel(this);      
    }

    public IActionResult Index()
    {
        var model = _aquecimento.ResetarStatusMicroondasSeInvalido();
        // attach available custom programs read from JSON file
        model.CustomPrograms = CustomProgramRepository.GetAll();
        return View(model);
    }

#region Funções básicas
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
    public IActionResult ExecuteCustomProgram(int programId)
    {
        var programas = CustomProgramRepository.GetAll();
        var programa = programas.FirstOrDefault(p => p.Id == programId.ToString());
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
            return Json(new { 
                                success = false, 
                                html = RenderPartialViewToString("_CustomProgramModal", input) 
                            });
        }

        //recuperando os registros do JSON
        var existing = CustomProgramRepository.GetAll();

        //validar nome e caractere de progresso para evitar duplicidade
        if (existing.Any(p => p.Nome.Equals(input.Nome, StringComparison.OrdinalIgnoreCase)))
        {
            ModelState.AddModelError("Nome", "Já existe um programa com esse nome.");
            return Json(new { 
                                success = false, 
                                html = RenderPartialViewToString("_CustomProgramModal", input) 
                            });
        }

        if (TipoAquecimentoConstants.CaracteresProgressoReservados.Contains(input.CaractereProgresso))
        {
            ModelState.AddModelError("CaractereProgresso", "Este caractere de progressão é reservado para os modos predefinidos.");
            return Json(new { 
                                success = false, 
                                html = RenderPartialViewToString("_CustomProgramModal", input) 
                            });
        }

        if (existing.Any(p => p.CaractereProgresso == input.CaractereProgresso))
        {
            ModelState.AddModelError("CaractereProgresso", "Este caractere de progressão já está cadastrado.");
            return Json(new { 
                                success = false, 
                                html = RenderPartialViewToString("_CustomProgramModal", input) 
                            });
        }

        CustomProgramRepository.Add(input);
        return Json(new { success = true });
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
                ModelState.AddModelError(error.Key, error.Value);
            }
            return Json(new { success = false, html = RenderPartialViewToString("_CustomProgramModal", input) });
        }

        var existing = CustomProgramRepository.GetAll();
        if (existing.Any(p => p.Id != input.Id && p.Nome.Equals(input.Nome, StringComparison.OrdinalIgnoreCase)))
        {
            ModelState.AddModelError("Nome", "Já existe um programa com esse nome.");
            return Json(new { success = false, html = RenderPartialViewToString("_CustomProgramModal", input) });
        }
        if (existing.Any(p => p.Id != input.Id && p.CaractereProgresso == input.CaractereProgresso))
        {
            ModelState.AddModelError("CaractereProgresso", "Este caractere de progressão já está cadastrado.");
            return Json(new { success = false, html = RenderPartialViewToString("_CustomProgramModal", input) });
        }

        CustomProgramRepository.Update(input);
        return Json(new { success = true });
    }

    [HttpPost]
    [IgnoreAntiforgeryToken]
    public IActionResult DeletePrograma(string id)
    {
        CustomProgramRepository.Delete(id);
        return Json(new { success = true });
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
