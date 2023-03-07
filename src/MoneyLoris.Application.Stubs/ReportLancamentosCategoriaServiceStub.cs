using MoneyLoris.Application.Reports.LancamentosCategoria;
using MoneyLoris.Application.Reports.LancamentosCategoria.Dto;

namespace MoneyLoris.Application.Stubs;
public class ReportLancamentosCategoriaServiceStub : IReportLancamentosCategoriaService
{
    public ICollection<CategoriaGroupItemtoDto> RelatorioLancamentosPorCategoria(int mes, int ano, int quantidade)
    {
        var ret = new List<CategoriaGroupItemtoDto>
        {
            new CategoriaGroupItemtoDto
            {
                catNome = "Receitas",
                jan = 7367.98M,
                fev = 6598.88M,
                Items = new List<CategoriaGroupItemtoDto> {
                    new CategoriaGroupItemtoDto
                    {
                        catId = 5,
                        catNome = "Salário",
                        jan = 5367.98M,
                        fev = 4598.88M
                    },
                    new CategoriaGroupItemtoDto
                    {
                        catId = 6,
                        catNome = "Reembolso",
                        jan = 1000.00M,
                        fev = 1000
                    }
                }
            },
            new CategoriaGroupItemtoDto
            {
                catNome = "Despesas",
                jan = -5000M,
                fev = -6269.89M,
                Items = new List<CategoriaGroupItemtoDto> {
                    new CategoriaGroupItemtoDto
                    {
                        catId = 1,
                        catNome = "Moradia",
                        jan = -2000M,
                        fev = -2488.99M,
                        Items = new List<CategoriaGroupItemtoDto>
                        {
                            new CategoriaGroupItemtoDto
                            {
                                catId = 1,
                                catNome = "Moradia",
                                subId = 11,
                                subNome = null,
                                jan = -109.90M,
                                fev = -836.22M,
                            },
                            new CategoriaGroupItemtoDto
                            {
                                catId = 1,
                                catNome = "Moradia",
                                subId = 12,
                                subNome = "Condomínio",
                                jan = -655.39M,
                                fev = -575.12M,
                            },
                            new CategoriaGroupItemtoDto
                            {
                                catId = 1,
                                catNome = "Moradia",
                                subId = 13,
                                subNome = "Luz",
                                jan = -150.44M,
                                fev = -238.33M,
                            }
                        }
                    },
                    new CategoriaGroupItemtoDto
                    {
                        catId = 2,
                        catNome = "Transporte",
                        jan = 372.44M,
                        fev = 238.66M,
                        Items = new List<CategoriaGroupItemtoDto>
                        {
                            new CategoriaGroupItemtoDto
                            {
                                catId = 2,
                                catNome = "Transporte",
                                subId = 21,
                                subNome = null,
                                jan = -40.00M,
                                fev = -45.00M,
                            },
                            new CategoriaGroupItemtoDto
                            {
                                catId = 2,
                                catNome = "Transporte",
                                subId = 22,
                                subNome = "Aplicativo",
                                jan = -89.95M,
                                fev = -78.44M,
                            },
                        }
                    },
                    new CategoriaGroupItemtoDto
                    {
                        catId = 9,
                        catNome = "Outras Despesas",
                        jan = 25.99M,
                        fev = 147.87M
                    }
                }
            }
        };

        return ret;
    }
}
