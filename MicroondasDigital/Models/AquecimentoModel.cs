using MicroondasDigital.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace MicroondasDigital.Models;

public class AquecimentoModel
{
    private readonly Controller _parentController;
    private static string FormatTempo(int tempo) =>  $"{tempo / 60:D2}:{tempo % 60:D2}";

    public AquecimentoModel(Controller controller)
    {
        _parentController = controller;
    }

#region Rotinas de Processamento e Gravação do Estado do Microondas

    private HttpContext GetContext()
    {
        return _parentController.HttpContext;
    }

    private MicroondasViewModel GetStatusMicroondas()
    {
        return SessionHelper.GetStatusMicroondas(GetContext());
    }

    private void SetStatusMicroondas(MicroondasViewModel state)
    {
        SessionHelper.SetStatusMicroondas(GetContext(), state);
    }

    private void ClearStatusMicroondas()
    {
        SessionHelper.ClearStatusMicroondas(GetContext());
    }

    private void AlterarStatusAquecimento(StatusAquecimento status)
    {
        MicroondasViewModel model = GetStatusMicroondas();

        model.StatusEnum = status;

        SetStatusMicroondas(model);
    }

    private object GetViewObjectStatusMicroondas(MicroondasViewModel model)
    {
        return new 
        {
            tempoFormatado = FormatTempo(model.TempoAquecimento),
            display        = model.Display,
            status         = model.Status,
            isRunning      = model.IsRunning(),
            isPaused       = model.IsPaused(),
            instrucoes     = model.Instrucoes,
            ismodoAquecimentoPadrao = model.ModoAquecimento == TipoAquecimento.Padrao
        };
    }

    public MicroondasViewModel ResetarStatusMicroondasSeInvalido()
    {
        MicroondasViewModel model = GetStatusMicroondas();

        if( (model.TempoAquecimento <= 0) &&
            (model.StatusEnum == StatusAquecimento.Parado) )
        {
            ClearStatusMicroondas();            
        }

        return model;
    }

    public void IniciarAquecimento(TipoAquecimento modoAquecimento, int tempoAquecimento, int potencia, char caractereProgresso)
    {
        MicroondasViewModel model =  new MicroondasViewModel
        {
            ModoAquecimento = modoAquecimento,
            TempoAquecimento = tempoAquecimento,
            Potencia = potencia,
            CaractereProgresso = caractereProgresso,
            Display = string.Empty,
            StatusEnum = StatusAquecimento.Aquecendo
        };
        
        SetStatusMicroondas(model);
    }

    public void IniciarModoAquecimentoPredefinido(TipoAquecimento ModoAquecimento)
    {
        IniciarAquecimento(ModoAquecimento, 
                           TipoAquecimentoConstants.GetTempoAquecimento(ModoAquecimento),
                           TipoAquecimentoConstants.GetPotencia(ModoAquecimento), 
                           TipoAquecimentoConstants.GetProgressChar(ModoAquecimento));
    }

    public void IniciarAquecimentoRapido()
    {
        MicroondasViewModel model = GetStatusMicroondas();

        switch(model.StatusEnum) 
        {
            case StatusAquecimento.Parado:
                model.TempoAquecimento = Constants.TempoAquecimentoInicioRapido;
                model.Potencia = Constants.PotenciaPadrao;
                model.Display = "";
                break;

            case StatusAquecimento.Aquecendo:
            case StatusAquecimento.Pausado:
                model.TempoAquecimento += Constants.TempoAquecimentoInicioRapido;
                break;

            default:
                break;
        }

        model.StatusEnum = StatusAquecimento.Aquecendo;

        SetStatusMicroondas(model);
    }    

    public void PausarAquecimento()
    {
        AlterarStatusAquecimento(StatusAquecimento.Pausado);
    }

    public void RetomarAquecimento()
    {
        AlterarStatusAquecimento(StatusAquecimento.Aquecendo);
    }    

    public void PararAquecimento()
    {
        ClearStatusMicroondas();
    }

    public object? ObterJSONStatusMicroondas()
    {
        MicroondasViewModel model = GetStatusMicroondas();

        return GetViewObjectStatusMicroondas(model); 
    }

    public object? ExecutarProgressoAquecimento()
    {
        MicroondasViewModel model = GetStatusMicroondas();
        
        if ( model.IsRunning() && 
             (!model.IsPaused()) && 
             (model.TempoAquecimento > 0) )
        {
            model.TempoAquecimento--;
            model.Display += model.GetProgressStep() + ' ';
            
            //Console.WriteLine($"Tick OK: novo tempo={model.TempoAquecimento}");
            
            if (model.TempoAquecimento <= 0)
            {
                model.StatusEnum = StatusAquecimento.Parado;
                model.Display += " Aquecimento concluído!";
            }
        }
        
        SetStatusMicroondas(model);

        return GetViewObjectStatusMicroondas(model);
    }    
#endregion

}