using MoneyLoris.Application.Business.Categorias.Dtos;
using MoneyLoris.Application.Business.Lancamentos;
using MoneyLoris.Application.Business.Lancamentos.Dtos;
using MoneyLoris.Application.Common.Base;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Stubs;
public class LancamentoServiceStub : ServiceBase, ILancamentoService
{
    public Task<Result<Pagination<ICollection<LancamentoListItemDto>>>> Pesquisar(LancamentoFiltroDto filtro)
    {
        var lancs = MockList();

        //filtro
        lancs = aplicarFiltro(filtro, lancs);

        //totalizador
        var totalFiltrado = lancs.Count();

        //corte de pagina
        lancs = lancs.Skip(filtro.ResultsPerPage * (filtro.CurrentPage - 1)).Take(filtro.ResultsPerPage).ToList();

        return TaskSuccess(dataPage: lancs, total: totalFiltrado);
    }

    public Task<Result<LancamentoBalancoDto>> ObterBalanco(LancamentoFiltroDto filtro)
    {
        var lancs = MockList();

        //filtro
        lancs = aplicarFiltro(filtro, lancs);

        var receita = lancs.Where(l => l.Tipo == TipoLancamento.Receita).Sum(l => l.Valor);
        var despesa = lancs.Where(l => l.Tipo == TipoLancamento.Despesa).Sum(l => l.Valor);
        var balanco = receita + despesa;

        var ret = new LancamentoBalancoDto
        {
            Receitas = receita,
            Despesas = despesa,
            Balanco = balanco
        };

        return TaskSuccess(ret);
    }

    private ICollection<LancamentoListItemDto> aplicarFiltro(LancamentoFiltroDto filtro, ICollection<LancamentoListItemDto> lancs)
    {
        //filtros
        if (!String.IsNullOrWhiteSpace(filtro.Descricao))
            lancs = lancs.Where(c => c.Descricao.ToUpper().Contains(filtro.Descricao.ToUpper())).ToList();

        if (filtro.DataInicio.HasValue)
            lancs = lancs.Where(c => c.Data >= filtro.DataInicio).ToList();

        if (filtro.DataFim.HasValue)
            lancs = lancs.Where(c => c.Data <= filtro.DataFim).ToList();

        return lancs;
    }

    public Task<Result<ICollection<LancamentoSugestaoDto>>> ObterSugestoesDespesas(string termoBusca)
    {
        ICollection<LancamentoSugestaoDto> list = null!;

        //se termo busca não foi informado, traz recentes. Senão, filtra pelo termo

        if (String.IsNullOrWhiteSpace(termoBusca))
        {
            list =
            new List<LancamentoSugestaoDto> {
                new LancamentoSugestaoDto {
                    Descricao = "Marmitime",
                    Categoria = new CategoriaListItemDto {
                        CategoriaNome = "Alimentação", CategoriaId = 601,
                        SubcategoriaNome = "Marmita", SubcategoriaId = 701
                    }
                },
                new LancamentoSugestaoDto {
                    Descricao = "Mc Donald's",
                    Categoria = new CategoriaListItemDto {
                        CategoriaNome = "Alimentação", CategoriaId = 601,
                        SubcategoriaNome = "Lanche", SubcategoriaId = 702
                    }
                },
                new LancamentoSugestaoDto {
                    Descricao = "Pacheco",
                    Categoria = new CategoriaListItemDto {
                        CategoriaNome = "Farmácia", CategoriaId = 602
                    }
                },
                new LancamentoSugestaoDto {
                    Descricao = "Recarga Celular",
                    Categoria = new CategoriaListItemDto {
                        CategoriaNome = "Assinaturas", CategoriaId = 603
                    }
                }
            };
        }
        else
        {
            list =
            new List<LancamentoSugestaoDto> {
                new LancamentoSugestaoDto {
                    Descricao = "Posto Shell",
                    Categoria = new CategoriaListItemDto {
                        CategoriaNome = "Veículo", CategoriaId = 604,
                        SubcategoriaNome = "Combustível", SubcategoriaId = 708
                    }
                },
                new LancamentoSugestaoDto {
                    Descricao = "Extrabom",
                    Categoria = new CategoriaListItemDto {
                        CategoriaNome = "Alimentação", CategoriaId = 601,
                        SubcategoriaNome = "Mercado", SubcategoriaId = 705
                    }
                },
                new LancamentoSugestaoDto {
                    Descricao = "Faxina",
                    Categoria = new CategoriaListItemDto {
                        CategoriaNome = "Moradia", CategoriaId = 605,
                        SubcategoriaNome = "Limpeza", SubcategoriaId = 706
                    }
                }
            };
        }


        return TaskSuccess(list);
    }

    public Task<Result<ICollection<LancamentoSugestaoDto>>> ObterSugestoesReceitas(string termoBusca)
    {
        //se termo busca não foi informado, traz recentes. Senão, filtra pelo termo

        ICollection<LancamentoSugestaoDto> list =
            new List<LancamentoSugestaoDto> {
                new LancamentoSugestaoDto {
                    Descricao = "Salário",
                    Categoria = new CategoriaListItemDto {
                        CategoriaNome = "Salário", CategoriaId = 610
                    }
                },
                new LancamentoSugestaoDto {
                    Descricao = "Reembolso Jurandir",
                    Categoria = new CategoriaListItemDto {
                        CategoriaNome = "Reembolso", CategoriaId = 611
                    }
                },
                new LancamentoSugestaoDto {
                    Descricao = "Rendimento Poupança",
                    Categoria = new CategoriaListItemDto {
                        CategoriaNome = "Investimentos", CategoriaId = 612
                    }
                },
            };

        return TaskSuccess(list);
    }

    public async Task<Result<int>> InserirReceita(LancamentoCadastroDto lancamento)
    {
        return await TaskSuccess((123, "Receita lançada com sucesso."));
    }

    public async Task<Result<int>> InserirDespesa(LancamentoCadastroDto lancamento)
    {
        return await TaskSuccess((123, "Despesa lançada com sucesso."));
    }

    public async Task<Result<LancamentoCadastroDto>> ObterLancamento(int id)
    {
        var ret = new LancamentoCadastroDto { 
            Id = 502,
            Data = SystemTime.Today().AddDays(-1),
            Tipo = TipoLancamento.Despesa,
            IdMeioPagamento = 302,  //nubank
            IdCategoria= 101,  //Alimentação
            IdSubcategoria = 213,  //Lanche
            Descricao = "Mc Donald's",
            Valor = 49.50M  //Importante: tem que voltar sempre positivo
        };
        return await TaskSuccess(ret);
    }

    public async Task<Result<int>> AlterarLancamento(LancamentoCadastroDto lancamento)
    {
        return await TaskSuccess((123, "Lançamento alterado com sucesso."));
    }

    public async Task<Result<int>> ExcluirLancamento(int id)
    {
        return await TaskSuccess((123, "Lançamento excluído com sucesso."));
    }


    public async Task<Result<int>> InserirTransferenciaEntreContas(TransferenciaInsertDto transferencia)
    {
        return await TaskSuccess((123, "Transferência lançada com sucesso."));
    }

    public async Task<Result<int>> InserirPagamentoFatura(TransferenciaInsertDto transferencia)
    {
        return await TaskSuccess((123, "Pagamento de Fatura lançada com sucesso."));
    }

    public async Task<Result<int>> AlterarTransferencia(TransferenciaUpdateDto lancamento)
    {
        return await TaskSuccess((123, "Transferência alterada com sucesso."));
    }

    public async Task<Result<int>> ExcluirTransferencia(int idLancamentoOrigem)
    {
        return await TaskSuccess((123, "Transferência excluída com sucesso."));
    }


    private ICollection<LancamentoListItemDto> MockList()
    {
        var list = new List<LancamentoListItemDto>
        {
            new LancamentoListItemDto
            {
                Id = 501,
                Data = SystemTime.Today(),
                Tipo = TipoLancamento.Despesa,
                Operacao = OperacaoLancamento.LancamentoSimples,
                MeioPagamentoNome = "Caixa",
                MeioPagamentoTipo = TipoMeioPagamento.ContaCorrente,
                MeioPagamentoCor = "0369B9",
                Categoria = "Alimentação",
                Subcategoria = "Marmita",
                Descricao = "Marmitime",
                Valor = -318.00M
            },
            new LancamentoListItemDto
            {
                Id = 502,
                Data = SystemTime.Today(),
                Tipo = TipoLancamento.Despesa,
                Operacao = OperacaoLancamento.LancamentoSimples,
                MeioPagamentoNome = "Nubank",
                MeioPagamentoTipo = TipoMeioPagamento.CartaoCredito,
                MeioPagamentoCor = "820AD1",
                Categoria = "Alimentação",
                Subcategoria = "Lanche",
                Descricao = "Mc Donald's",
                Valor = -49.50M
            },
            new LancamentoListItemDto
            {
                Id = 503,
                Data = SystemTime.Today().AddDays(-1),
                Tipo = TipoLancamento.Despesa,
                Operacao = OperacaoLancamento.LancamentoSimples,
                MeioPagamentoNome = "Nubank",
                MeioPagamentoTipo = TipoMeioPagamento.CartaoCredito,
                MeioPagamentoCor = "820AD1",
                Categoria = "Farmácia",
                Subcategoria = null!,
                Descricao = "Pacheco",
                Valor = -17.49M
            },
            new LancamentoListItemDto
            {
                Id = 504,
                Data = SystemTime.Today().AddDays(-2),
                Tipo = TipoLancamento.Receita,
                Operacao = OperacaoLancamento.LancamentoSimples,
                MeioPagamentoNome = "PicPay",
                MeioPagamentoTipo = TipoMeioPagamento.CarteiraDigital,
                MeioPagamentoCor = "11C56E",
                Categoria = "Repasse",
                Subcategoria = null!,
                Descricao = "Reembolso Jurandir",
                Valor = 275.00M
            },
            new LancamentoListItemDto
            {
                Id = 505,
                Data = SystemTime.Today().AddDays(-3),
                Tipo = TipoLancamento.Despesa,
                Operacao = OperacaoLancamento.LancamentoSimples,
                MeioPagamentoNome = "Banco do Brasil",
                MeioPagamentoTipo = TipoMeioPagamento.ContaCorrente,
                MeioPagamentoCor = "FCF800",
                Categoria = "Assinaturas",
                Subcategoria = null!,
                Descricao = "Recarga Celular",
                Valor = -50.00M
            },
            new LancamentoListItemDto
            {
                Id = 506,
                Data = SystemTime.Today().AddDays(-5),
                Tipo = TipoLancamento.Receita,
                Operacao = OperacaoLancamento.Transferencia,
                MeioPagamentoNome = "Banco do Brasil",
                MeioPagamentoTipo = TipoMeioPagamento.ContaCorrente,
                MeioPagamentoCor = "FCF800",
                Categoria = "Transferência",
                Subcategoria = null!,
                Descricao = "Transferência de Caixa",
                Valor = 550.00M
            },
            new LancamentoListItemDto
            {
                Id = 507,
                Data = SystemTime.Today().AddDays(-5),
                Tipo = TipoLancamento.Despesa,
                Operacao = OperacaoLancamento.Transferencia,
                MeioPagamentoNome = "Caixa",
                MeioPagamentoTipo = TipoMeioPagamento.ContaCorrente,
                MeioPagamentoCor = "0369B9",
                Categoria = "Transferência",
                Subcategoria = null!,
                Descricao = "Transferência para Banco do Brasil",
                Valor = -550.00M
            },
            new LancamentoListItemDto
            {
                Id = 508,
                Data = SystemTime.Today().AddDays(-10),
                Tipo = TipoLancamento.Receita,
                Operacao = OperacaoLancamento.Transferencia,
                MeioPagamentoNome = "Nubank",
                MeioPagamentoTipo = TipoMeioPagamento.CartaoCredito,
                MeioPagamentoCor = "820AD1",
                Categoria = "Pagamento de Fatura",
                Subcategoria = null!,
                Descricao = "Pagamento Fatura Nubank via Caixa",
                Valor = 1895.44M
            },
            new LancamentoListItemDto
            {
                Id = 509,
                Data = SystemTime.Today().AddDays(-10),
                Tipo = TipoLancamento.Despesa,
                Operacao = OperacaoLancamento.Transferencia,
                MeioPagamentoNome = "Caixa",
                MeioPagamentoTipo = TipoMeioPagamento.ContaCorrente,
                MeioPagamentoCor = "0369B9",
                Categoria = "Pagamento de Fatura",
                Subcategoria = null!,
                Descricao = "Pagamento Fatura Nubank via Caixa",
                Valor = -1895.44M
            },

        };

        for (int i = 1; i <= 50; i++)
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
