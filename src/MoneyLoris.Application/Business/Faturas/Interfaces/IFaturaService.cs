using MoneyLoris.Application.Business.Faturas.Dtos;
using MoneyLoris.Application.Business.Lancamentos.Dtos;
using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Business.Faturas.Interfaces;
public interface IFaturaService
{
    Task<Result<Pagination<ICollection<LancamentoListItemDto>>>> Pesquisar(FaturaFiltroDto filtro);
    Task<Result<FaturaInfoDto>> ObterInfo(FaturaAnoMesFiltroDto filtro);

    Task<Result<FaturaSelecaoDto>> ObterFaturaEmAberto(int IdCartao);
    Task<Result<ICollection<FaturaSelecaoDto>>> ObterFaturasSelecao(int IdCartao);

    Task<Fatura> ObterOuCriarFatura(MeioPagamento cartao, int mes, int ano);
}
