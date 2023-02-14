namespace MoneyLoris.Application.Shared;
public class BusinessException : Exception
{
    public string ErrorCode { get; private set; } = default!;

    public BusinessException(string code, string message)
        : base(message)
    {
        ErrorCode = code;
    }

    public BusinessException(string code, string message, Exception inner)
        : base(message, inner)
    {
        ErrorCode = code;
    }
}
