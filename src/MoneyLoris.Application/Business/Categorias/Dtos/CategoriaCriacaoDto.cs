using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Domain.Enums;

namespace MoneyLoris.Application.Business.Categorias.Dtos;
public class CategoriaCriacaoDto
{
    public string Nome { get; set; } = default!;
    public byte? Ordem { get; set; }
    public TipoLancamento Tipo { get; set; }

    public CategoriaCriacaoDto()
    {
    }

    public CategoriaCriacaoDto(Categoria categoria)
    {
        Nome = categoria.Nome;
        Ordem = categoria.Ordem;
        Tipo = categoria.Tipo;
    }
}
