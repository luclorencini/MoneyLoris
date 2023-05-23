namespace MoneyLoris.Application.Reports.LancamentosCategoria.Dto;
public class CategoriaReportItemDto
{
    public string Descricao { get; set; } = default!;
    public int? IdCategoria { get; set; }
    public int? IdSubcategoria { get; set; }

    public decimal? Valor01 { get; set; }
    public decimal? Valor02 { get; set; }
    public decimal? Valor03 { get; set; }
    public decimal? Valor04 { get; set; }
    public decimal? Valor05 { get; set; }
    public decimal? Valor06 { get; set; }
    public decimal? Valor07 { get; set; }
    public decimal? Valor08 { get; set; }
    public decimal? Valor09 { get; set; }
    public decimal? Valor10 { get; set; }
    public decimal? Valor11 { get; set; }
    public decimal? Valor12 { get; set; }

    public ICollection<CategoriaReportItemDto>? Items { get; set; }
}
