namespace MicroondasCliente.Exceptions;

public class BusinessException : Exception
{
    public string? UserMessage { get; }

    public BusinessException(string message) : base(message)
    {
        UserMessage = message;
    }

    public BusinessException(string message, Exception inner)
        : base(message, inner)
    {
        UserMessage = message;
    }
}
