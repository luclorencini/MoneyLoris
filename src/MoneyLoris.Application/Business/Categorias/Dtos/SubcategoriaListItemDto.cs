namespace MoneyLoris.Application.Business.Categorias.Dtos;
public class SubcategoriaListItemDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = default!;
    public int? Ordem { get; set; }
}
