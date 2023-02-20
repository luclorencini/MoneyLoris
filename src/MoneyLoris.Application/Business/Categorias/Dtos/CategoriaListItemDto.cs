namespace MoneyLoris.Application.Business.Categorias.Dtos;
public class CategoriaListItemDto
{
    public int CategoriaId { get; set; }
    public string CategoriaNome { get; set; } = default!;
    public int? SubcategoriaId { get; set; }
    public string SubcategoriaNome { get; set; } = default!;
}
