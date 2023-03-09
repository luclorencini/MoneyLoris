namespace MoneyLoris.Application.Reports.LancamentosCategoria;
public class CategoriaReportItemDto
{
    public string Descricao { get; set; } = default!;
    public int? IdCategoria { get; set; }
    public int? IdSubcategoria { get; set; }

    public decimal? Valor01 { get; set; }
    public decimal? Valor02 { get; set; }
    public decimal? Valor03 { get; set; }

    public ICollection<CategoriaReportItemDto>? Items { get; set; }
}
