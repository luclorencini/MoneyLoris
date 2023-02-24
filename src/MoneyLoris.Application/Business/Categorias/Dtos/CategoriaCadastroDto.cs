using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Domain.Enums;

namespace MoneyLoris.Application.Business.Categorias.Dtos;
public class CategoriaCadastroDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = default!;
    public byte? Ordem { get; set; }
    public TipoLancamento Tipo { get; set; }

    public CategoriaCadastroDto()
    {
    }

    public CategoriaCadastroDto(Categoria categoria)
    {
        Id = categoria.Id;
        Nome = categoria.Nome;
        Ordem = categoria.Ordem;
        Tipo = categoria.Tipo;
    }
}
