using MicroondasDigital.Models.Enums;

namespace MicroondasDigital.Models;

public class MicroondasViewModel : BaseViewModel
{
    public int TempoAquecimento { get; set; }
    public int Potencia { get; set; } = Constants.PotenciaPadrao;
    public char CaractereProgresso { get; set; } = TipoAquecimentoConstants.GetProgressChar(TipoAquecimento.Padrao);
    public StatusAquecimento StatusEnum { get; set; } = StatusAquecimento.Parado;
    public string Status => StatusConstants.GetDescricao(StatusEnum);    
    public TipoAquecimento ModoAquecimento { get; set; } = TipoAquecimento.Padrao;
    public string Modo => TipoAquecimentoConstants.GetDescricao(ModoAquecimento);
    public string Instrucoes {get; set; } = string.Empty;
    public string Display { get; set; } = string.Empty;

protected override IEnumerable<ValidationResult> GetValidations()
    {
        return
        [
            new ValidationResult
            {
                IsValid = TempoAquecimento >= 1,
                Field = nameof(TempoAquecimento),
                Message = "O Tempo não pode ser menor que 1."
            },
            new ValidationResult
            {
                IsValid = (ModoAquecimento == TipoAquecimento.Padrao) && (TempoAquecimento <= Constants.TempoAquecimentoMaxInputManual),
                Field = nameof(TempoAquecimento),
                Message = "O Tempo não pode passar de 2min (120 seg)."
            },
            new ValidationResult
            {
                IsValid = (Potencia >= 1) && (Potencia <= Constants.PotenciaMax),
                Field = nameof(Potencia),
                Message = "A Potência deve ficar entre 1 e 10."
            }
        ];
    }

    public bool IsRunning()
    {
        return new[]{StatusAquecimento.Aquecendo, 
                    StatusAquecimento.Pausado}.Contains(StatusEnum);
    }

    public bool IsPaused()
    {
        return StatusEnum == StatusAquecimento.Pausado;
    }


    public string GetProgressStep()
    {
        return new string(CaractereProgresso, Potencia);
    }
}
