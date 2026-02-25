using MicroondasDigital.Models.Enums;

namespace MicroondasDigital.Models;

public class MicroondasViewModel
{
    public int Tempo { get; set; }
    public int Potencia { get; set; } = 10;
    public bool IsRunning { get; set; }
    public bool IsPaused { get; set; }
    public StatusAquecimento StatusEnum { get; set; } = StatusAquecimento.Parado;
    public string Status => StatusConstants.GetDescricao(StatusEnum);
    public string Display { get; set; } = "";
}
