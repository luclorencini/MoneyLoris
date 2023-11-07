using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Reports.LancamentosCategoria.Dto;

namespace MoneyLoris.Application.Reports.LancamentosCategoria;
public interface IReportLancamentosCategoriaRepository
{
    List<CategoriaQueryResultItemtoDto> RelatorioLancamentosPorCategoria(int idUsuario, TipoLancamento tipo, ReportLancamentoFilterDto filtro);

    Task<int> DetalheTotalRegistros(int idUsuario, ReportLancamentoDetalheFilterDto filtro);
    Task<ICollection<Lancamento>> DetalhePaginado(int idUsuario, ReportLancamentoDetalheFilterDto filtro);

    Task<decimal> DetalheSomatorio(int idUsuario, ReportLancamentoDetalheFilterDto filtro);
}
