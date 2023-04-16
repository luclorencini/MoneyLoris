using MoneyLoris.Application.Business.Lancamentos.Dtos;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Business.Lancamentos.Interfaces;
public interface ILancamentoConsultaService
{
    Task<Result<Pagination<ICollection<LancamentoListItemDto>>>> Pesquisar(LancamentoFiltroDto filtro);
    Task<Result<LancamentoBalancoDto>> ObterBalanco(LancamentoFiltroDto filtro);

    Task<Result<ICollection<LancamentoSugestaoDto>>> ObterSugestoesDespesas(string termoBusca);
    Task<Result<ICollection<LancamentoSugestaoDto>>> ObterSugestoesReceitas(string termoBusca);

}
