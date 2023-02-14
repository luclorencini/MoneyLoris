using MoneyLoris.Application.Business.Categorias.Dtos;
using MoneyLoris.Application.Business.Categorias.Interfaces;
using MoneyLoris.Application.Domain;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Stubs;
public class CategoriaServiceStub : ICategoriaService
{
    public async Task<Result<ICollection<CategoriaListItemDto>>> ListarCategoriasUsuario(Enums.TipoLancamento tipo)
    {
        List<CategoriaListItemDto> ret = null!;

        if (tipo == Enums.TipoLancamento.Despesa)
        {
            ret = new List<CategoriaListItemDto>()
            {
                new CategoriaListItemDto { Id = 101, Nome = "Alimentação", Ordem = 1,
                    Subcategorias = new List<SubcategoriaListItemDto>() {
                        new SubcategoriaListItemDto { Id = 211, Nome = "Restaurante", Ordem = 1 },
                        new SubcategoriaListItemDto { Id = 212, Nome = "Supermercado", Ordem = 2 },
                        new SubcategoriaListItemDto { Id = 213, Nome = "Lanche" },
                    }
                },
                new CategoriaListItemDto { Id = 102, Nome = "Moradia", Ordem = 2,
                    Subcategorias = new List<SubcategoriaListItemDto>() {
                        new SubcategoriaListItemDto { Id = 221, Nome = "Água", Ordem = 1 },
                        new SubcategoriaListItemDto { Id = 222, Nome = "Luz", Ordem = 2 },
                        new SubcategoriaListItemDto { Id = 223, Nome = "Condomínio" },
                    }
                },
                new CategoriaListItemDto { Id = 103, Nome = "Transporte", Ordem = 3 },
                new CategoriaListItemDto { Id = 104, Nome = "Pessoal",
                    Subcategorias = new List<SubcategoriaListItemDto>() {
                        new SubcategoriaListItemDto { Id = 241, Nome = "Academia", Ordem = 1 },
                        new SubcategoriaListItemDto { Id = 242, Nome = "Vestuário", Ordem = 2 }
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

        return ret;
    }
}
