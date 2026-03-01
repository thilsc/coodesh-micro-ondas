using System.Net;
using System.Text.Json;
using MicroondasDigital.Exceptions;
using MicroondasDigital.Utils;

namespace MicroondasDigital.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (BusinessException bex)
        {
            Logger.Log(bex);
            _logger.LogWarning(bex, "Erro de negócio.");

            await WriteErrorResponse(context, bex.UserMessage ?? "Erro de negócio.");
        }
        catch (Exception ex)
        {
            Logger.Log(ex);
            _logger.LogError(ex, "Erro não tratado.");

            await WriteErrorResponse(context, "Ocorreu um erro inesperado. Tente novamente.");
        }
    }

    private static async Task WriteErrorResponse(HttpContext context, string message)
    {
        var isAjax = (context.Request.Headers.XRequestedWith == "XMLHttpRequest") ||
                     (context.Request.Path.Value?.Contains("/Tick", StringComparison.OrdinalIgnoreCase) ?? false) ||
                     (context.Request.Path.Value?.Contains("/StatusAtual", StringComparison.OrdinalIgnoreCase) ?? false) ||
                     (context.Request.ContentType?.Contains("application/json") ?? false);

        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        if (isAjax)
        {
            context.Response.ContentType = "application/json; charset=utf-8";

            var payload = new
            {
                success = false,
                error = message
            };

            var json = JsonSerializer.Serialize(payload);

            await context.Response.WriteAsync(json);
        }
        else
        {
            context.Response.ContentType = "text/html; charset=utf-8";

            await context.Response.WriteAsync($"<h1>Erro</h1><p>{message}</p>");
        }
    }
}
