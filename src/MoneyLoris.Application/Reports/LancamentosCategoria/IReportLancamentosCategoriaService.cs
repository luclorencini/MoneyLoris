using MoneyLoris.Application.Reports.LancamentosCategoria.Dto;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Reports.LancamentosCategoria;
public interface IReportLancamentosCategoriaService
{
    Result<ICollection<CategoriaReportItemDto>> RelatorioLancamentosPorCategoria(ReportLancamentoFilterDto filtro);
}
