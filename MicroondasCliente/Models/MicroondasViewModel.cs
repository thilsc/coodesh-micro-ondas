using MicroondasCliente.Models.Enums;

namespace MicroondasCliente.Models;

public class MicroondasViewModel
{
    public int TempoAquecimento { get; set; }
    public string TempoFormatado { get; set; } = "00:00";
    public int Potencia { get; set; }
    public string Display { get; set; } = string.Empty;
    public string Status { get; set; } = "Parado";
    public string Instrucoes { get; set; } = string.Empty;
    public IList<AquecimentoCustomizadoModel> CustomPrograms { get; set; } = new List<AquecimentoCustomizadoModel>();
    public string MensagemDeErro { get; set; } = string.Empty;
    public StatusAquecimento StatusAtual { get; set; }
    public int PotenciaAtual { get; set; }
    public int TempoRestante { get; set; }

    public bool IsRunning()
    {
        return new[]{StatusAquecimento.Aquecendo, 
                    StatusAquecimento.Pausado}.Contains(StatusAtual);
    }

    public bool IsPaused()
    {
        return StatusAtual == StatusAquecimento.Pausado;
    }    
}
