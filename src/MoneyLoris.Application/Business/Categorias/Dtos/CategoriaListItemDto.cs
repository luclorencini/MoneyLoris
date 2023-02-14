namespace MoneyLoris.Application.Business.Categorias.Dtos;
public class CategoriaListItemDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = default!;
    public int? Ordem { get; set; }
    public ICollection<SubcategoriaListItemDto> Subcategorias { get; set; } = new List<SubcategoriaListItemDto>();
}
