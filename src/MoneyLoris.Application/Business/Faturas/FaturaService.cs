using MoneyLoris.Application.Business.Faturas.Dtos;
using MoneyLoris.Application.Business.Faturas.Interfaces;
using MoneyLoris.Application.Business.Lancamentos.Dtos;
using MoneyLoris.Application.Common.Base;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Business.Faturas;
public class FaturaService : ServiceBase, IFaturaService
{
    public Task<Result<FaturaInfoDto>> ObterInfo(FaturaFiltroDto filtro)
    {
        throw new NotImplementedException();
    }

    public Task<Result<Pagination<ICollection<LancamentoListItemDto>>>> Pesquisar(FaturaFiltroDto filtro)
    {
        throw new NotImplementedException();
    }
}
