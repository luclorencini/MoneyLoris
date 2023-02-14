using MoneyLoris.Application.Business.Categorias.Dtos;
using MoneyLoris.Application.Domain;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Business.Categorias.Interfaces;
public interface ICategoriaService
{
    Task<Result<ICollection<CategoriaListItemDto>>> ListarCategoriasUsuario(Enums.TipoLancamento tipo);
}
