using Microsoft.AspNetCore.Mvc;

namespace MicroondasCliente.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        var token = HttpContext.Session.GetString("JWTToken");
        bool isAuth = !string.IsNullOrEmpty(token);
        
        ViewBag.IsAuthenticated = isAuth;
        
        return View();
    }
}
