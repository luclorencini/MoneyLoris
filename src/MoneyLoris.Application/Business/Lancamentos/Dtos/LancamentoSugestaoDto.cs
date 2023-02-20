using MoneyLoris.Application.Business.Categorias.Dtos;

namespace MoneyLoris.Application.Business.Lancamentos.Dtos;
public class LancamentoSugestaoDto
{
    public string Descricao { get; set; } = default!;
    public CategoriaListItemDto Categoria { get; set; } = default!;
}
