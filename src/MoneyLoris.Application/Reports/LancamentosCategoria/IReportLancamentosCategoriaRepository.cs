using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Reports.LancamentosCategoria.Dto;

namespace MoneyLoris.Application.Reports.LancamentosCategoria;
public interface IReportLancamentosCategoriaRepository
{
    List<CategoriaQueryResultItemtoDto> RelatorioLancamentosPorCategoria(int idUsuario, TipoLancamento tipo, int mes, int ano, int quantidade);

    Task<int> DetalheTotalRegistros(ReportLancamentoDetalheFilterDto filtro, int idUsuario);
    Task<ICollection<Lancamento>> DetalhePaginado(ReportLancamentoDetalheFilterDto filtro, int idUsuario);

    Task<decimal> DetalheSomatorio(ReportLancamentoDetalheFilterDto filtro, int idUsuario);
}
