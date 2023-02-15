using MoneyLoris.Application.Business.Categorias.Dtos;
using MoneyLoris.Application.Business.Categorias.Interfaces;
using MoneyLoris.Application.Common.Base;
using MoneyLoris.Application.Domain;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Stubs;
public class CategoriaServiceStub : ServiceBase, ICategoriaService
{
    public async Task<Result<ICollection<CategoriaListItemDto>>> ListarCategoriasUsuario(Enums.TipoLancamento tipo)
    {
        ICollection<CategoriaListItemDto> ret = null!;

        if (tipo == Enums.TipoLancamento.Despesa)
        {
            ret = new List<CategoriaListItemDto>()
            {
                new CategoriaListItemDto { Id = 101, Nome = "Alimentação", Ordem = 1,
                    Subcategorias = new List<SubcategoriaListItemDto>() {
                        new SubcategoriaListItemDto { Id = 211, IdCategoria = 101, Nome = "Restaurante", Ordem = 1 },
                        new SubcategoriaListItemDto { Id = 212, IdCategoria = 101, Nome = "Supermercado", Ordem = 2 },
                        new SubcategoriaListItemDto { Id = 213, IdCategoria = 101, Nome = "Lanche" },
                    }
                },
                new CategoriaListItemDto { Id = 102, Nome = "Moradia", Ordem = 2,
                    Subcategorias = new List<SubcategoriaListItemDto>() {
                        new SubcategoriaListItemDto { Id = 221, IdCategoria = 102, Nome = "Água", Ordem = 1 },
                        new SubcategoriaListItemDto { Id = 222, IdCategoria = 102, Nome = "Luz", Ordem = 2 },
                        new SubcategoriaListItemDto { Id = 223, IdCategoria = 102, Nome = "Condomínio" },
                    }
                },
                new CategoriaListItemDto { Id = 103, Nome = "Transporte", Ordem = 3 },
                new CategoriaListItemDto { Id = 104, Nome = "Pessoal",
                    Subcategorias = new List<SubcategoriaListItemDto>() {
                        new SubcategoriaListItemDto { Id = 241, IdCategoria = 104, Nome = "Academia", Ordem = 1 },
                        new SubcategoriaListItemDto { Id = 242, IdCategoria = 104, Nome = "Vestuário", Ordem = 2 }
                    }},
            };

        }
        else
        {
            ret = new List<CategoriaListItemDto>()
            {
                new CategoriaListItemDto { Id = 201, Nome = "Salário", Ordem = 1 },
                new CategoriaListItemDto { Id = 202, Nome = "Empréstimos", Ordem = 2 },
                new CategoriaListItemDto { Id = 203, Nome = "Reembolso", Ordem = 3 },
                new CategoriaListItemDto { Id = 204, Nome = "Investimentos", Ordem = 4 },
                new CategoriaListItemDto { Id = 205, Nome = "Outras Receitas" }
            };
        }

        return await TaskSuccess(ret);
    }


    public async Task<Result<CategoriaCadastroDto>> ObterCategoria(int id)
    {
        var ret = new CategoriaCadastroDto { Id = 101, Nome = "Alimentação", Ordem = 1 };
        return await TaskSuccess(ret);
    }

    public async Task<Result<int>> InserirCategoria(CategoriaCadastroDto model)
    {
        return await TaskSuccess(123);
    }

    public async Task<Result<int>> AlterarCategoria(CategoriaCadastroDto model)
    {
        return await TaskSuccess(123);
    }

    public async Task<Result<int>> ExcluirCategoria(int id)
    {
        return await TaskSuccess(123);
    }


    public async Task<Result<SubcategoriaCadastroDto>> ObterSubcategoria(int id)
    {
        var ret = new SubcategoriaCadastroDto { Id = 211, IdCategoria = 101, Nome = "Restaurante", Ordem = 1 };
        return await TaskSuccess(ret);
    }

    public async Task<Result<int>> InserirSubcategoria(SubcategoriaCadastroDto model)
    {
        return await TaskSuccess(456);
    }

    public async Task<Result<int>> AlterarSubcategoria(SubcategoriaCadastroDto model)
    {
        return await TaskSuccess(456);
    }

    public async Task<Result<int>> ExcluirSubcategoria(int id)
    {
        return await TaskSuccess(456);
    }
}
