using System.Text.Json.Serialization;

namespace MoneyLoris.Application.Shared;
public class Result
{
    public bool Ok { get; protected set; }

    public string? ErrorCode { get; protected set; }

    public string? Message { get; protected set; }

    public bool IsOk() => Ok;

    public Result(string? message = null)
    {
        Message = message;
        Ok = true;
    }

    public Result(string errorCode, string? errorMessage)
    {
        ErrorCode = errorCode;
        Message = errorMessage;
        Ok = false;
    }

    [JsonConstructor]
    public Result(string? errorCode, string? message, bool ok)
    {
        ErrorCode = errorCode;
        Message = message;
        Ok = ok;
    }


    //sucesso
    public static Result Success()
    {
        return new Result();
    }
}

public sealed class Result<T> : Result
{

    public T? Value { get; private set; }

    public Result(T value, string? message = null)
    {
        Message = message;
        Value = value;
        Ok = true;
    }

    public Result(string errorCode, string? errorMessage) : base(errorCode, errorMessage) { }


    [JsonConstructor]
    public Result(string? errorCode, string? message, bool ok, T value)
    {
        ErrorCode = errorCode;
        Message = message;
        Ok = ok;
        Value = value;
    }


    //sucesso
    public static implicit operator Result<T>(T value)
    {
        var r = new Result<T>(value);
        return r;
    }

    public static implicit operator Result<T>((T value, string message) data)
    {
        var r = new Result<T>(data.value, data.message);
        return r;
    }
}
