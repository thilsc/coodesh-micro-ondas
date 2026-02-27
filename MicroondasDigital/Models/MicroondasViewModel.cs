using MicroondasDigital.Models.Enums;

namespace MicroondasDigital.Models;

public class MicroondasViewModel
{
    private List<string> _log = [];
    //private bool _loadingFromJSON = false;
    public int TempoAquecimento { get; set; }
    public int Potencia { get; set; } = Constants.PotenciaPadrao;
    public char CaractereProgresso { get; set; } = TipoAquecimentoConstants.GetProgressChar(TipoAquecimento.Padrao);
    public StatusAquecimento StatusEnum { get; set; } = StatusAquecimento.Parado;
    public string Status => StatusConstants.GetDescricao(StatusEnum);    
    public TipoAquecimento ModoAquecimento { get; set; } = TipoAquecimento.Padrao;
    public string Modo => TipoAquecimentoConstants.GetDescricao(ModoAquecimento);
    public string Instrucoes => TipoAquecimentoConstants.GetInstrucoes(ModoAquecimento);    
    public string Display { get; set; } = string.Empty;

    public MicroondasViewModel()
    {
        //implementação do create
    }

    public string GetErrorLog()
    {
        return string.Join(Environment.NewLine, _log);
    }

    public bool Validate()
    {
        _log.Clear();

        if(TempoAquecimento < 1) 
        {
            _log.Add("O tempo informado não pode ser menor que 1");
        }

        if(TempoAquecimento > Constants.TempoAquecimentoMaxInputManual)
        {
            _log.Add("O tempo informado não pode passar de 2 minutos.");
        }

        if( (Potencia < 1) || (Potencia > Constants.PotenciaPadrao) )
        {
            _log.Add("A Potência deve ficar entre 1 e 10.");
        }

        return _log.Count == 0;
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
