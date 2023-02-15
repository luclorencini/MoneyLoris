using MoneyLoris.Application.Business.Categorias.Dtos;
using MoneyLoris.Application.Business.Categorias.Interfaces;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Business.Categorias;
public class CategoriaService : ICategoriaService
{
    public async Task<Result<ICollection<CategoriaListItemDto>>> ListarCategoriasUsuario(TipoLancamento tipo)
    {
        //TODO
        throw new NotImplementedException();
    }


    public async Task<Result<CategoriaCadastroDto>> ObterCategoria(int id)
    {
        //TODO
        throw new NotImplementedException();
    }

    public async Task<Result<int>> InserirCategoria(CategoriaCadastroDto model)
    {
        //TODO
        throw new NotImplementedException();
    }
    public async Task<Result<int>> AlterarCategoria(CategoriaCadastroDto model)
    {
        //TODO
        throw new NotImplementedException();
    }

    public async Task<Result<int>> ExcluirCategoria(int id)
    {
        //TODO
        throw new NotImplementedException();
    }


    public Task<Result<SubcategoriaCadastroDto>> ObterSubcategoria(int id)
    {
        //TODO
        throw new NotImplementedException();
    }

    public Task<Result<int>> InserirSubcategoria(SubcategoriaCadastroDto model)
    {
        //TODO
        throw new NotImplementedException();
    }

    public Task<Result<int>> AlterarSubcategoria(SubcategoriaCadastroDto model)
    {
        //TODO
        throw new NotImplementedException();
    }

    public Task<Result<int>> ExcluirSubcategoria(int id)
    {
        //TODO
        throw new NotImplementedException();
    }
}
