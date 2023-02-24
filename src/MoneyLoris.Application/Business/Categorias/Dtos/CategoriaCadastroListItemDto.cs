using MoneyLoris.Application.Domain.Entities;

namespace MoneyLoris.Application.Business.Categorias.Dtos;
public class CategoriaCadastroListItemDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = default!;
    public int? Ordem { get; set; }
    public ICollection<SubcategoriaCadastroListItemDto> Subcategorias { get; set; } = new List<SubcategoriaCadastroListItemDto>();

    public CategoriaCadastroListItemDto()
    {
    }

    public CategoriaCadastroListItemDto(Categoria categoria)
    {
        Id = categoria.Id;
        Nome = categoria.Nome;
        Ordem = categoria.Ordem;
        Subcategorias = new List<SubcategoriaCadastroListItemDto>(); //TODO - carregar as subs
    }
}
