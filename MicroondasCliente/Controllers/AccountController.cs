using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using MicroondasCliente.Models;

namespace MicroondasCliente.Controllers;

public class AccountController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AccountController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet]
    public IActionResult Login()
    {
        // Se já tem token, vai pra Home
        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("JWTToken")))
            return RedirectToAction("Index", "Home");
            
        return View(new LoginRequest());
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginRequest model)
    {
        if (!ModelState.IsValid) return View(model);

        var client = _httpClientFactory.CreateClient("MicroondasApi");
        var jsonContent = new StringContent(JsonSerializer.Serialize(new { 
            username = model.Username, 
            password = model.Password 
        }), Encoding.UTF8, "application/json");

        var response = await client.PostAsync("MicroondasApi/auth/login", jsonContent);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (result != null && result.IsAuthenticated)
            {
                // Guarda o token na sessão
                HttpContext.Session.SetString("JWTToken", result.Token);
                return RedirectToAction("Index", "Home");
            }
        }

        ModelState.AddModelError(string.Empty, "Usuário ou senha inválidos.");
        return View(model);
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Remove("JWTToken");
        return RedirectToAction("Login");
    }
}
