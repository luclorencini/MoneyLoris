namespace MoneyLoris.Application.Reports.LancamentosCategoria.Dto;
public class CategoriaQueryResultItemtoDto
{
    public int? catId { get; set; }
    public string? catNome { get; set; } = default!;
    public int? catOrdem { get; set; }
    public int? subId { get; set; }
    public string? subNome { get; set; } = default!;
    public int? subOrdem { get; set; }

    public decimal? jan { get; set; }
    public decimal? fev { get; set; }
    public decimal? mar { get; set; }


    public ICollection<CategoriaQueryResultItemtoDto>? Items { get; set; }
}
