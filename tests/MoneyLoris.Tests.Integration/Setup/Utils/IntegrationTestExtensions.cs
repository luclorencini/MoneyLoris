using System.Net.Http.Json;
using MoneyLoris.Application.Shared;
using Xunit.Sdk;

namespace MoneyLoris.Tests.Integration.Setup.Utils;
public static class IntegrationTestExtensions
{
    public static async Task<T> ConverteResultOk<T>(this HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();

        var retorno = await response.Content.ReadFromJsonAsync<Result<T>>();

        if (!retorno!.IsOk())
            throw new XunitException($"{retorno.ErrorCode} - {retorno.Message}");

        Assert.True(retorno!.IsOk(), "retornou Ok = False, deveria retornar True");

        return retorno.Value!;
    }

    public static async Task<Result> AssertResultNotOk(this HttpResponseMessage response, string errorCode)
    {
        response.EnsureSuccessStatusCode();

        var retorno = await response.Content.ReadFromJsonAsync<Result>();

        if (!retorno!.IsOk() && retorno.ErrorCode == ErrorCodes.SystemError)
            throw new XunitException($"{retorno.ErrorCode} - {retorno.Message}");

        Assert.False(retorno!.IsOk(), "retornou Ok = True, deveria retornar False");

        Assert.Equal(errorCode, retorno.ErrorCode);

        return retorno;
    }


    //private static async Task<Result<T>?> ReadFromJsonCustomAsync<T>(this HttpResponseMessage response)
    //{
    //    var ms = new MemoryStream();

    //    try
    //    {
    //        response.Content.CopyTo(ms, default, default);
    //        var ret = await response.Content.ReadFromJsonAsync<Result<T>>();
    //        return ret;
    //    }
    //    catch
    //    {
    //        var x = new ByteArrayContent(ms.ToArray());
    //        var retEx = await x.ReadFromJsonAsync<Result>();

    //        if (retEx == null)
    //            throw new Exception($"Result NULL");

    //        throw new Exception($"{retEx.ErrorCode} - {retEx.Message}");
    //    }
    //}
}
