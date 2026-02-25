using System.Text.Json;

namespace MicroondasDigital.Models;

public static class SessionHelper
{
    private const string MICROWAVE_STATE_KEY = "StatusMicroondas";

    public static MicroondasViewModel GetStatusMicroondas(HttpContext context)
    {
        var json = context.Session.GetString(MICROWAVE_STATE_KEY);
        return json != null 
            ? JsonSerializer.Deserialize<MicroondasViewModel>(json) ?? new MicroondasViewModel()
            : new MicroondasViewModel();
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