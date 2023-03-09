using MoneyLoris.Application.Reports.LancamentosCategoria.Dto;

namespace MoneyLoris.Application.Reports.LancamentosCategoria;
public interface IReportLancamentosCategoriaService
{
    ICollection<CategoriaReportItemDto> RelatorioLancamentosPorCategoria(int mes, int ano, int quantidade);
}
