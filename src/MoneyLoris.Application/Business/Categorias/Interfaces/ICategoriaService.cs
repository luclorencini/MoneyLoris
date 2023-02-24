using MoneyLoris.Application.Business.Categorias.Dtos;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Business.Categorias.Interfaces;
public interface ICategoriaService
{
    Task<Result<ICollection<CategoriaCadastroListItemDto>>> ListarCategoriasUsuario(TipoLancamento tipo);

    Task<Result<CategoriaCadastroDto>> ObterCategoria(int id);
    Task<Result<int>> InserirCategoria(CategoriaCadastroDto dto);
    Task<Result<int>> AlterarCategoria(CategoriaCadastroDto dto);
    Task<Result<int>> ExcluirCategoria(int id);

    Task<Result<SubcategoriaCadastroDto>> ObterSubcategoria(int id);
    Task<Result<int>> InserirSubcategoria(SubcategoriaCadastroDto dto);
    Task<Result<int>> AlterarSubcategoria(SubcategoriaCadastroDto dto);
    Task<Result<int>> ExcluirSubcategoria(int id);

    Task<Result<ICollection<CategoriaListItemDto>>> ObterCategoriasUsuario(TipoLancamento tipo);
}
