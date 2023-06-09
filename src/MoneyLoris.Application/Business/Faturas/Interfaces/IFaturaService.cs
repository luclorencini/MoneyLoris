using MoneyLoris.Application.Business.Faturas.Dtos;
using MoneyLoris.Application.Business.Lancamentos.Dtos;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Business.Faturas.Interfaces;
public interface IFaturaService
{
    Task<Result<Pagination<ICollection<LancamentoListItemDto>>>> Pesquisar(FaturaFiltroDto filtro);
    Task<Result<FaturaInfoDto>> ObterInfo(FaturaFiltroDto filtro);
}
