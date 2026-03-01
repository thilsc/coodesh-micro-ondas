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
        return View(_aquecimento.ResetarStatusMicroondasSeInvalido());
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
        var programas = new List<AquecimentoCustomizadoModel>
        {
            new() {
                Id = "1",
                Nome = "Batata e beterraba",
                Alimento = "Batata e beterraba",
                Tempo = 180,
                Potencia = 8,
                CaractereProgresso = '*',
                Instrucoes = "Corte a batata e a beterraba em pedaços médios para garantir um aquecimento uniforme."
            },
            new() {
                Id = "2",
                Nome = "Vegetais congelados",
                Alimento = "Vegetais congelados",
                Tempo = 240,
                Potencia = 7,
                CaractereProgresso = 'w',
                Instrucoes = "Corte os vegetais congelados em pedaços médios para garantir um aquecimento uniforme."
            },
            new() {
                Id = "3",
                Nome = "Peixe",
                Alimento = "Peixe",
                Tempo = 300,
                Potencia = 6,
                CaractereProgresso = '?',
                Instrucoes = "Corte o peixe em pedaços médios para garantir um aquecimento uniforme."
            }
        };

        if (programas.FirstOrDefault(p => p.Id == programId.ToString()) is AquecimentoCustomizadoModel programa)
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

    /*
    [HttpGet]
    public IActionResult ListaProgramas()
    {
        var list = CustomProgramRepository.GetAll();
        return PartialView("_CustomProgramListModal", list);
    }

    // GET form de criação
    [HttpGet]
    public IActionResult CriarProgramaCustomizado()
    {
        var model = new AquecimentoCustomizadoModel();
        return PartialView("_CustomProgramModal", model);
    }

    // POST criação
    [HttpPost]
    [IgnoreAntiforgeryToken]
    public IActionResult CriarProgramaCustomizado(AquecimentoCustomizadoModel input)
    {
        if (!input.Validate())
        {
            ModelState.AddModelError("", input.GetErrorLog());
            return Json(new { success = false, html = RenderPartialViewToString("_CustomProgramModal", input) });
        }

        var existing = CustomProgramRepository.GetAll();
        if (existing.Any(p => p.Nome.Equals(input.Nome, StringComparison.OrdinalIgnoreCase)))
        {
            ModelState.AddModelError("Nome", "Já existe um programa com esse nome.");
            return Json(new { success = false, html = RenderPartialViewToString("_CustomProgramModal", input) });
        }
        if (existing.Any(p => p.CaractereProgresso == input.CaractereProgresso))
        {
            ModelState.AddModelError("CaractereProgresso", "Este caractere de progressão já está cadastrado.");
            return Json(new { success = false, html = RenderPartialViewToString("_CustomProgramModal", input) });
        }

        CustomProgramRepository.Add(input);
        return Json(new { success = true });
    }

    // GET form de edição
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
            ModelState.AddModelError("", input.GetErrorLog());
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

    // exclusão
    [HttpPost]
    [IgnoreAntiforgeryToken]
    public IActionResult DeletePrograma(string id)
    {
        CustomProgramRepository.Delete(id);
        return Json(new { success = true });
    }

    // iniciar com um programa customizado
    [HttpPost]
    [IgnoreAntiforgeryToken]
    public IActionResult UsarPrograma(string id)
    {
        var item = CustomProgramRepository.GetAll().FirstOrDefault(p => p.Id == id);
        if (item != null)
        {
            _aquecimento.IniciarAquecimento(TipoAquecimento.Padrao,
                                           item.Tempo,
                                           item.Potencia,
                                           item.CaractereProgresso);
        }
        return Json(new { success = true });
    }

    // helper para renderizar partial em string (usado para responder erros via ajax)
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
    */
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
