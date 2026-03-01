using System.Text;

namespace MicroondasDigital.Utils;

public static class Logger
{
    private static readonly object _lock = new();

    private const string LogDirectory = "Logs";
    private const string LogFileName = "exceptions.log";

    public static void Log(Exception ex)
    {
        try
        {
            if (!Directory.Exists(LogDirectory))
                Directory.CreateDirectory(LogDirectory);

            var path = Path.Combine(LogDirectory, LogFileName);
            var sb = new StringBuilder();

            sb.AppendLine("==================================================");
            sb.AppendLine($"Data/Hora : {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
            sb.AppendLine($"Tipo      : {ex.GetType().FullName}");
            sb.AppendLine($"Mensagem  : {ex.Message}");

            if (ex.InnerException != null)
            {
                sb.AppendLine($"InnerType : {ex.InnerException.GetType().FullName}");
                sb.AppendLine($"InnerMsg  : {ex.InnerException.Message}");
            }

            sb.AppendLine("StackTrace:");
            sb.AppendLine(ex.StackTrace ?? "(sem stack trace)");
            sb.AppendLine();

            lock (_lock)
            {
                File.AppendAllText(path, sb.ToString());
            }
        }
        catch
        {
            // Logger deve ser silencioso
        }
    }
}
