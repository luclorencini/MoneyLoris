using MoneyLoris.Application.Business.Lancamentos.Dtos;
using MoneyLoris.Application.Common.Base;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Reports.LancamentosCategoria;
using MoneyLoris.Application.Reports.LancamentosCategoria.Dto;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Stubs;
public class ReportLancamentosCategoriaServiceStub : ServiceBase, IReportLancamentosCategoriaService
{

    public Result<ICollection<CategoriaReportItemDto>> LancamentosPorCategoriaConsolidado(ReportLancamentoFilterDto filtro)
    {
        var ret = new List<CategoriaReportItemDto>
        {
            new CategoriaReportItemDto
            {
                Descricao = "Receitas",
                Valor01 = 7367.98M,
                Valor02 = 6598.88M,
                Items = new List<CategoriaReportItemDto> {
                    new CategoriaReportItemDto
                    {
                        IdCategoria = 5,
                        Descricao = "Salário",
                        Valor01 = 5367.98M,
                        Valor02 = 4598.88M
                    },
                    new CategoriaReportItemDto
                    {
                        IdCategoria = 6,
                        Descricao = "Reembolso",
                        Valor01 = 1000.00M,
                        Valor02 = 1000
                    }
                }
            },
            new CategoriaReportItemDto
            {
                Descricao = "Despesas",
                Valor01 = -5000M,
                Valor02 = -6269.89M,
                Items = new List<CategoriaReportItemDto> {
                    new CategoriaReportItemDto
                    {
                        IdCategoria = 1,
                        Descricao = "Moradia",
                        Valor01 = -2000M,
                        Valor02 = -2488.99M,
                        Items = new List<CategoriaReportItemDto>
                        {
                            new CategoriaReportItemDto
                            {
                                IdCategoria = 1,
                                IdSubcategoria = 11,
                                Descricao = null!,
                                Valor01 = -109.90M,
                                Valor02 = -836.22M,
                            },
                            new CategoriaReportItemDto
                            {
                                IdCategoria = 1,
                                IdSubcategoria = 12,
                                Descricao = "Condomínio",
                                Valor01 = -655.39M,
                                Valor02 = -575.12M,
                            },
                            new CategoriaReportItemDto
                            {
                                IdCategoria = 1,
                                IdSubcategoria = 13,
                                Descricao = "Luz",
                                Valor01 = -150.44M,
                                Valor02 = -238.33M,
                            }
                        }
                    },
                    new CategoriaReportItemDto
                    {
                        IdCategoria = 2,
                        Descricao = "Transporte",
                        Valor01 = 372.44M,
                        Valor02 = 238.66M,
                        Items = new List<CategoriaReportItemDto>
                        {
                            new CategoriaReportItemDto
                            {
                                IdCategoria = 2,
                                IdSubcategoria = 21,
                                Descricao = null!,
                                Valor01 = -40.00M,
                                Valor02 = -45.00M,
                            },
                            new CategoriaReportItemDto
                            {
                                IdCategoria = 2,
                                IdSubcategoria = 22,
                                Descricao = "Aplicativo",
                                Valor01 = -89.95M,
                                Valor02 = -78.44M,
                            },
                        }
                    },
                    new CategoriaReportItemDto
                    {
                        IdCategoria = 9,
                        Descricao = "Outras Despesas",
                        Valor01 = 25.99M,
                        Valor02 = 147.87M
                    }
                }
            }
        };

        return ret;
    }

    public Task<Result<Pagination<ICollection<LancamentoListItemDto>>>> PesquisarDetalhe(ReportLancamentoDetalheFilterDto filtro)
    {
        var lancs = MockList();

        //totalizador
        var totalFiltrado = lancs.Count();

        //corte de pagina
        lancs = lancs.Skip(filtro.ResultsPerPage * (filtro.CurrentPage - 1)).Take(filtro.ResultsPerPage).ToList();

        return TaskSuccess(dataPage: lancs, total: totalFiltrado);
    }

    private ICollection<LancamentoListItemDto> MockList()
    {
        var list = new List<LancamentoListItemDto>();

        for (int i = 1; i <= 60; i++)
        {
            list.Add(new LancamentoListItemDto
            {
                Id = 600 + i,
                Data = DateTime.Now.AddDays(-(10 + i)),
                Tipo = (i % 2 == 0 ? TipoLancamento.Despesa : TipoLancamento.Receita),
                Operacao = OperacaoLancamento.LancamentoSimples,
                MeioPagamentoNome = "PicPay",
                MeioPagamentoTipo = TipoMeioPagamento.CarteiraDigital,
                MeioPagamentoCor = "11C56E",
                Categoria = "Outras Despesas",
                Subcategoria = null!,
                Descricao = $"Compra {i}",
                Valor = (i % 2 == 0 ? -(25.00M + i) : (25.00M + i))
            });
        }

        return list;
    }
}
