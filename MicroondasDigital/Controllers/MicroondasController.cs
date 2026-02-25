using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using MicroondasDigital.Models;
using MicroondasDigital.Models.Enums;

namespace MicroondasDigital.Controllers;

public class MicroondasController : Controller
{
    public IActionResult Index()
    {
        var model = SessionHelper.GetStatusMicroondas(HttpContext);
        
        if (model.IsRunning && model.Tempo <= 0)
        {
            SessionHelper.ClearStatusMicroondas(HttpContext);
            model.StatusEnum = StatusAquecimento.Parado;
        }
        
        var aquecimento = new AquecimentoModel(model);
        ViewBag.TempoFormatado = aquecimento.TempoFormatado;
        ViewBag.StatusAtual = model.StatusEnum;

        return View(model);
    }

    [HttpGet]
    public IActionResult GetTempoAtual()
    {
        var model = SessionHelper.GetStatusMicroondas(HttpContext);
        using var aquecimento = new AquecimentoModel(model);
        return Json(new { tempoFormatado = aquecimento.TempoFormatado });
    }

    [HttpPost]
    public IActionResult Iniciar(MicroondasViewModel input)
    {
        var model = SessionHelper.GetStatusMicroondas(HttpContext);
        
        using var aquecimento = new AquecimentoModel(model);
        model.Tempo = input.Tempo;
        model.Potencia = input.Potencia;
        
        try
        {
            aquecimento.Iniciar();
            SessionHelper.SetStatusMicroondas(HttpContext, model);
        }
        catch
        {
            ModelState.AddModelError("", "Tempo (1-120s) ou potência (1-10) inválido");
            return View("Index", model);
        }
        
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult InicioRapido()
    {
        var model = SessionHelper.GetStatusMicroondas(HttpContext);
        model.TempoSegundos = 30;
        model.Potencia = 10;
        
        var aquecimento = new AquecimentoModel(model);
        aquecimento.Iniciar();
        
        SessionHelper.SetStatusMicroondas(HttpContext, model);
        
        return RedirectToAction("Index");
    }    

    [HttpPost]
    public IActionResult Pausar()
    {
        var model = SessionHelper.GetStatusMicroondas(HttpContext);
        var aquecimento = new AquecimentoModel(model);
        aquecimento.Pausar();
        
        SessionHelper.SetStatusMicroondas(HttpContext, model);

        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Retomar()
    {
        var model = SessionHelper.GetStatusMicroondas(HttpContext);
        using var aquecimento = new AquecimentoModel(model);
        aquecimento.Retomar();
        SessionHelper.SetStatusMicroondas(HttpContext, model);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Cancelar()
    {
        var model = SessionHelper.GetStatusMicroondas(HttpContext);
        using var aquecimento = new AquecimentoModel(model);
        aquecimento.Cancelar();
        SessionHelper.ClearStatusMicroondas(HttpContext);
        return RedirectToAction("Index");
    }
}
