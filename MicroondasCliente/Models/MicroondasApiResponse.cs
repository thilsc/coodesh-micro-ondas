public class MicroondasApiResponse
{
    public string TempoFormatado { get; set; } = string.Empty;
    public string Display { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Instrucoes { get; set; } = string.Empty;
    public bool IsRunning { get; set; } = false;
    public bool IsPaused { get; set; } = false;
    public bool IsModoAquecimentoPadrao { get; set; } = true;
}