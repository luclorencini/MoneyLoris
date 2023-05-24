using MoneyLoris.Application.Business.Lancamentos.Dtos;
using MoneyLoris.Application.Reports.LancamentosCategoria.Dto;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Reports.LancamentosCategoria;
public interface IReportLancamentosCategoriaService
{
    Result<ICollection<CategoriaReportItemDto>> LancamentosPorCategoriaConsolidado(ReportLancamentoFilterDto filtro);

    Task<Result<Pagination<ICollection<LancamentoListItemDto>>>> PesquisarDetalhe(ReportLancamentoDetalheFilterDto filtro);
    Task<Result<decimal>> ObterDetalheSomatorio(ReportLancamentoDetalheFilterDto filtro);
}
