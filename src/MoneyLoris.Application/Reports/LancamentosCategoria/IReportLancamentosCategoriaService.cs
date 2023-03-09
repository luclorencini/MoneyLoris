using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Reports.LancamentosCategoria;
public interface IReportLancamentosCategoriaService
{
    Result<ICollection<CategoriaReportItemDto>> RelatorioLancamentosPorCategoria(int mes, int ano, int quantidade);
}
