using MoneyLoris.Application.Domain.Entities;

namespace MoneyLoris.Application.Business.Categorias.Dtos;
public class SubcategoriaCadastroDto
{
    public int Id { get; set; }
    public int IdCategoria { get; set; }
    public string Nome { get; set; } = default!;
    public byte? Ordem { get; set; }

    public SubcategoriaCadastroDto()
    {
    }

    public SubcategoriaCadastroDto(Subcategoria subcategoria)
    {
        Id = subcategoria.Id;
        IdCategoria = subcategoria.IdCategoria;
        Nome = subcategoria.Nome;
        Ordem = subcategoria.Ordem;
    }
}
