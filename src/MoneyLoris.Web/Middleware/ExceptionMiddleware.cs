using System.IO.Hashing;
using System.Net;
using System.Net.Mime;
using System.Text;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Web.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger logger)
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
        catch (Exception ex)
        {
            context.Response.ContentType = MediaTypeNames.Application.Json;
            context.Response.StatusCode = (int)HttpStatusCode.OK;

            string code = null!;
            string message = null!;

            var codException = GerarCodigoErro();

            if (ex is BusinessException)
            {
                code = ((BusinessException)ex).ErrorCode;
                message = ex.Message;
            }
            else
            {
                code = ErrorCodes.SystemError;
                message = $"Ocorreu um erro inesperado. [{codException}]";

                //só loga erros não esperados
                _logger.LogError(ex, $"{codException}");
            }

            var rm = new Result(code, message);

            await context.Response.WriteAsJsonAsync(rm);
        }
    }

    private string GerarCodigoErro()
    {
        byte[] hash = Crc32.Hash(Encoding.UTF8.GetBytes($"{DateTime.Now}"));
        string strBase = Convert.ToHexString(hash);

        var codigo = $"APP-{strBase}";
        return codigo;
    }
}
