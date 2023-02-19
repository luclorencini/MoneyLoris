namespace MoneyLoris.Application.Business.Lancamentos.Dtos;
public class LancamentoSugestaoDto
{
    public string Descricao { get; set; } = default!;
    public string CategoriaNome { get; set; } = default!;
    public int CategoriaId { get; set; }
    public string SubcategoriaNome { get; set; } = default!;
    public int? SubcategoriaId { get; set; }
}
