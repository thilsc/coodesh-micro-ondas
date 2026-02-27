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
            ModelState.AddModelError("", input.GetErrorLog());
            return View("Index", input); 
        }
        else
        {
            _aquecimento.IniciarAquecimento(TipoAquecimento.Padrao, 
                                            input.TempoAquecimento, 
                                            input.Potencia, 
                                            TipoAquecimentoConstants.GetProgressChar(TipoAquecimento.Padrao));
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
