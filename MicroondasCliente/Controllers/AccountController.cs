using Microsoft.AspNetCore.Mvc;
using MicroondasCliente.Models;
using MicroondasCliente.Utils;

namespace MicroondasCliente.Controllers;

public class AccountController : Controller
{
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var loginRequest = new LoginRequest { Username = model.Username, Password = model.Password };
            var response = await ApiResponseHelper.ObterRetornoEndpointApiPost<LoginResponse>("auth/login", loginRequest);

            if (response?.Token != null)
            {
                HttpContext.Session.SetString("JWTToken", response.Token);
                return RedirectToAction("Index", "Microondas");
            }

            ModelState.AddModelError(string.Empty, "Login inválido");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"Erro ao conectar: {ex.Message}");
        }

        return View(model);
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}
