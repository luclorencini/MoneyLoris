namespace MoneyLoris.Application.Reports.LancamentosCategoria.Dto;
public class CategoriaQueryResultItemtoDto
{
    public int? catId { get; set; }
    public string? catNome { get; set; } = default!;
    public int? catOrdem { get; set; }
    public int? subId { get; set; }
    public string? subNome { get; set; } = default!;
    public int? subOrdem { get; set; }

    public decimal? val01 { get; set; }
    public decimal? val02 { get; set; }
    public decimal? val03 { get; set; }
    public decimal? val04 { get; set; }
    public decimal? val05 { get; set; }
    public decimal? val06 { get; set; }
    public decimal? val07 { get; set; }
    public decimal? val08 { get; set; }
    public decimal? val09 { get; set; }
    public decimal? val10 { get; set; }
    public decimal? val11 { get; set; }
    public decimal? val12 { get; set; }

    public ICollection<CategoriaQueryResultItemtoDto>? Items { get; set; }
}
