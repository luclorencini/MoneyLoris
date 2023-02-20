namespace MoneyLoris.Application.Business.Categorias.Dtos;
public class CategoriaCadastroListItemDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = default!;
    public int? Ordem { get; set; }
    public ICollection<SubcategoriaCadastroListItemDto> Subcategorias { get; set; } = new List<SubcategoriaCadastroListItemDto>();
}
