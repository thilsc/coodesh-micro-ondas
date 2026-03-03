using System.Text.Json;

namespace MicroondasDigital.Models;

public static class SessionHelper
{
    private const string MICROWAVE_STATE_KEY = "StatusMicroondas";

    public static MicroondasViewModel GetStatusMicroondas(HttpContext context)
    {
        var json = context.Session.GetString(MICROWAVE_STATE_KEY);

        if(json != null)
        {
            //Console.WriteLine($"Obtenção das informações da Sessão => {json}");

            var model = JsonSerializer.Deserialize<MicroondasViewModel>(json);

            return model ?? new MicroondasViewModel(); 
        }

        return new MicroondasViewModel();
    }

    public static void SetStatusMicroondas(HttpContext context, MicroondasViewModel state)
    {
        context.Session.SetString(MICROWAVE_STATE_KEY, JsonSerializer.Serialize(state));
    }

    public static void ClearStatusMicroondas(HttpContext context)
    {
        context.Session.Remove(MICROWAVE_STATE_KEY);
    }
}