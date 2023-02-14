using MoneyLoris.Application.Business.Categorias.Dtos;
using MoneyLoris.Application.Business.Categorias.Interfaces;
using MoneyLoris.Application.Domain;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Business.Categorias;
public class CategoriaService : ICategoriaService
{

    public async Task<Result<ICollection<CategoriaListItemDto>>> ListarCategoriasUsuario(Enums.TipoLancamento tipo)
    {
        //TODO
        throw new NotImplementedException();
    }
}
