using MicroondasDigital.Models.Enums;

namespace MicroondasDigital.Models;

public class AquecimentoModel : IDisposable
{
    private MicroondasViewModel _microondas;
    private System.Timers.Timer? _timer;
    private int _segundosRestantes;
    private bool _disposed = false;

    public bool IsRunning => _microondas.IsRunning;
    public bool IsPaused => _microondas.IsPaused;
    public int TempoRestante => _segundosRestantes;
    public string TempoFormatado => FormatTempo(_segundosRestantes);
    public string Status => _microondas.Status;
    public string Display => _microondas.Display;

    public StatusAquecimento StatusEnum => _microondas.StatusEnum;

    public AquecimentoModel(MicroondasViewModel microondas)
    {
        _microondas = microondas;
        _segundosRestantes = microondas.Tempo;
    }

    public void Iniciar()
    {
        if (!Validar()) throw new InvalidOperationException("Parâmetros inválidos");
        
        _microondas.IsRunning = true;
        _microondas.IsPaused = false;
        _segundosRestantes = _microondas.Tempo;
        _microondas.StatusEnum = StatusAquecimento.Aquecendo;
        _microondas.Display = "";

        _timer = new System.Timers.Timer(1000);
        _timer.Elapsed += OnTick;
        _timer.Start();
    }

    public void Pausar()
    {
        _microondas.IsPaused = true;
        _microondas.StatusEnum = StatusAquecimento.Pausado;
        _timer?.Stop();
    }

    public void Retomar()
    {
        _microondas.IsPaused = false;
        _microondas.StatusEnum = StatusAquecimento.Aquecendo;
        _timer?.Start();
    }

    public void Cancelar()
    {
        _timer?.Stop();
        _timer?.Dispose();
        _microondas.IsRunning = false;
        _microondas.IsPaused = false;
        _microondas.StatusEnum = StatusAquecimento.Cancelado;
        _microondas.Display = "";
        _segundosRestantes = 0;
    }

    public void Acrescentar30s()
    {
        if (_microondas.IsRunning && !_microondas.IsPaused)
        {
            _microondas.Tempo += 30;
            _segundosRestantes += 30;
            //_microondas.StatusEnum = StatusAquecimento.Aquecendo //_microondas.Status = "+30s adicionados";
        }
    }

    private string FormatTempo(int totalSeconds)
    {
        var minutes = totalSeconds / 60;
        var seconds = totalSeconds % 60;
        return $"{minutes:D2}:{seconds:D2}";
    }

    private bool Validar()
    {
        return _microondas.Tempo >= 1 && _microondas.Tempo <= 120 &&
               _microondas.Potencia >= 1 && _microondas.Potencia <= 10;
    }

    private void OnTick(object? sender, System.Timers.ElapsedEventArgs e)
    {
        if (_disposed) return;
        
        _segundosRestantes--;
        _microondas.Display += new string('.', _microondas.Potencia);
        
        if (_segundosRestantes <= 0)
        {
            Finalizar();
        }
    }

    private void Finalizar()
    {
        _timer?.Stop();
        _microondas.StatusEnum = StatusAquecimento.Concluido;
        _microondas.Display += " Aquecimento concluído!";
        _microondas.IsRunning = false;
    }

    public void Dispose()
    {
        _disposed = true;
        _timer?.Stop();
        _timer?.Dispose();
        GC.SuppressFinalize(this); 
    }
}