using MoneyLoris.Application.Business.Categorias.Dtos;
using MoneyLoris.Application.Domain;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Business.Categorias.Interfaces;
public interface ICategoriaService
{
    Task<Result<ICollection<CategoriaListItemDto>>> ListarCategoriasUsuario(Enums.TipoLancamento tipo);

    Task<Result<CategoriaCadastroDto>> ObterCategoria(int id);
    Task<Result<int>> InserirCategoria(CategoriaCadastroDto model);
    Task<Result<int>> AlterarCategoria(CategoriaCadastroDto model);
    Task<Result<int>> ExcluirCategoria(int id);

    Task<Result<SubcategoriaCadastroDto>> ObterSubcategoria(int id);
    Task<Result<int>> InserirSubcategoria(SubcategoriaCadastroDto model);
    Task<Result<int>> AlterarSubcategoria(SubcategoriaCadastroDto model);
    Task<Result<int>> ExcluirSubcategoria(int id);
}
