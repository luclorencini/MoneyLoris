using System.Net.Http.Json;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Tests.Integration.Setup.Utils;
public static class IntegrationTestExtensions
{
    public static async Task<T> AssertResultOk<T>(this HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();

        var retorno = await response.ReadFromJsonCustomAsync<T>();

        Assert.True(retorno!.IsOk(), "retornou Ok = False, deveria retornar True");

        return retorno.Value!;
    }

    public static async Task AssertResultNotOk(this HttpResponseMessage response, string errorCode)
    {
        response.EnsureSuccessStatusCode();

        var retorno = await response.ReadFromJsonCustomAsync<Result<object>>();

        Assert.False(retorno!.IsOk(), "retornou Ok = True, deveria retornar False");

        Assert.Equal(errorCode, retorno.ErrorCode);
    }


    private static async Task<Result<T>?> ReadFromJsonCustomAsync<T>(this HttpResponseMessage response)
    {
        var ms = new MemoryStream();

        try
        {
            response.Content.CopyTo(ms, default, default);
            var ret = await response.Content.ReadFromJsonAsync<Result<T>>();
            return ret;
        }
        catch
        {
            var x = new ByteArrayContent(ms.ToArray());
            var retEx = await x.ReadFromJsonAsync<Result<bool>>();

            if (retEx == null)
                throw new Exception($"Result NULL");

            throw new Exception($"{retEx.ErrorCode} - {retEx.Message}");
        }
    }
}
