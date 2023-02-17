using MoneyLoris.Application.Business.Lancamentos.Dtos;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Business.Lancamentos;
public interface ILancamentoService
{
    Task<Result<Pagination<ICollection<LancamentoListItemDto>>>> Pesquisar(LancamentoFiltroDto filtro);
    Task<Result<LancamentoBalancoDto>> ObterBalanco(LancamentoFiltroDto filtro);
}
