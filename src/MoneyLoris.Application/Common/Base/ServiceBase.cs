using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Common.Base;

public class ServiceBase
{
    public Result<Pagination<T>> Pagination<T>(T pagedData, long total)
    {
        var ret = new Result<Pagination<T>>(
            new Pagination<T>(pagedData, total)
        );
        return ret;
    }

    // utilitarios para Stubs, para retornar Tasks sem uma lógica que chama await

    public async Task<Result<T>> TaskSuccess<T>(T data)
    {
        await Task.Run(() => { });
        return data;
    }

    public async Task<Result<Pagination<T>>> TaskSuccess<T>(Pagination<T> pagination)
    {
        await Task.Run(() => { });
        return pagination;
    }

    public async Task<Result<Pagination<T>>> TaskSuccess<T>(T dataPage, long total)
    {
        await Task.Run(() => { });
        return Pagination(dataPage, total);
    }
}
