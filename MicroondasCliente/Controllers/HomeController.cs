using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MicroondasCliente.Models;

namespace MicroondasCliente.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        var token = HttpContext.Session.GetString("JWTToken");
        if (!string.IsNullOrEmpty(token))
        {
            // Se logado, vai direto pro Micro-ondas
            return RedirectToAction("Index", "Microondas");
        }
        
        // Se não logado, exibe a Home original avisando que está bloqueado
        return View();
    }
}
