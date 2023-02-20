using System.Collections.Generic;
using MoneyLoris.Application.Business.Categorias.Dtos;
using MoneyLoris.Application.Business.Categorias.Interfaces;
using MoneyLoris.Application.Common.Base;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Stubs;
public class CategoriaServiceStub : ServiceBase, ICategoriaService
{
    public async Task<Result<ICollection<CategoriaCadastroListItemDto>>> ListarCategoriasUsuario(TipoLancamento tipo)
    {
        ICollection<CategoriaCadastroListItemDto> ret = null!;

        if (tipo == TipoLancamento.Despesa)
        {
            ret = categoriasDespesa();
        }
        else
        {
            ret = categoriasReceita();
        }

        return await TaskSuccess(ret);
    }

    private ICollection<CategoriaCadastroListItemDto> categoriasReceita()
    {
        var ret = new List<CategoriaCadastroListItemDto>()
            {
                new CategoriaCadastroListItemDto { Id = 201, Nome = "Salário", Ordem = 1 },
                new CategoriaCadastroListItemDto { Id = 202, Nome = "Empréstimos", Ordem = 2 },
                new CategoriaCadastroListItemDto { Id = 203, Nome = "Reembolso", Ordem = 3 },
                new CategoriaCadastroListItemDto { Id = 204, Nome = "Investimentos", Ordem = 4 },
                new CategoriaCadastroListItemDto { Id = 205, Nome = "Outras Receitas" }
            };
        return ret;
    }

    private ICollection<CategoriaCadastroListItemDto> categoriasDespesa()
    {
        var ret = new List<CategoriaCadastroListItemDto>()
            {
                new CategoriaCadastroListItemDto { Id = 101, Nome = "Alimentação", Ordem = 1,
                    Subcategorias = new List<SubcategoriaCadastroListItemDto>() {
                        new SubcategoriaCadastroListItemDto { Id = 211, IdCategoria = 101, Nome = "Restaurante", Ordem = 1 },
                        new SubcategoriaCadastroListItemDto { Id = 212, IdCategoria = 101, Nome = "Supermercado", Ordem = 2 },
                        new SubcategoriaCadastroListItemDto { Id = 213, IdCategoria = 101, Nome = "Lanche" },
                    }
                },
                new CategoriaCadastroListItemDto { Id = 102, Nome = "Moradia", Ordem = 2,
                    Subcategorias = new List<SubcategoriaCadastroListItemDto>() {
                        new SubcategoriaCadastroListItemDto { Id = 221, IdCategoria = 102, Nome = "Água", Ordem = 1 },
                        new SubcategoriaCadastroListItemDto { Id = 222, IdCategoria = 102, Nome = "Luz", Ordem = 2 },
                        new SubcategoriaCadastroListItemDto { Id = 223, IdCategoria = 102, Nome = "Condomínio" },
                    }
                },
                new CategoriaCadastroListItemDto { Id = 103, Nome = "Transporte", Ordem = 3 },
                new CategoriaCadastroListItemDto { Id = 104, Nome = "Pessoal",
                    Subcategorias = new List<SubcategoriaCadastroListItemDto>() {
                        new SubcategoriaCadastroListItemDto { Id = 241, IdCategoria = 104, Nome = "Academia", Ordem = 1 },
                        new SubcategoriaCadastroListItemDto { Id = 242, IdCategoria = 104, Nome = "Vestuário", Ordem = 2 }
                    }},
            };

        return ret;
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

    public async Task<Result<ICollection<CategoriaListItemDto>>> ObterCategoriasUsuario(TipoLancamento tipo)
    {
        ICollection <CategoriaListItemDto> ret = new List<CategoriaListItemDto> ();

        ICollection <CategoriaCadastroListItemDto> lista = 
            (tipo == TipoLancamento.Despesa ? categoriasDespesa() : categoriasReceita());

        foreach (var c in lista)
        {
            ret.Add(new CategoriaListItemDto { IdCategoria = c.Id, Nome = c.Nome });

            foreach (var s in c.Subcategorias)
            {
                ret.Add(new CategoriaListItemDto { IdCategoria = c.Id, IdSubcategoria = s.Id, Nome = s.Nome });
            }

        }

        return await TaskSuccess(ret);
    }
}
