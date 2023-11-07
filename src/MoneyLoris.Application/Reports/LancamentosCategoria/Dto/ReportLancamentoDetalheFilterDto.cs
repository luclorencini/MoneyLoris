using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Reports.LancamentosCategoria.Dto;
public class ReportLancamentoDetalheFilterDto : PaginationFilter
{
    public int IdCategoria { get; set; }
    public int? IdSubcategoria { get; set; }
    public int Ano { get; set; }
    public int Mes { get; set; }
    public RegimeContabil Regime { get; set; }
    public bool TodosDaCategoria { get; set; }
}
