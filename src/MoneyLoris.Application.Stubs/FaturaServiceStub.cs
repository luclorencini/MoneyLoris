using System.Collections.Generic;
using MoneyLoris.Application.Business.Faturas.Dtos;
using MoneyLoris.Application.Business.Faturas.Interfaces;
using MoneyLoris.Application.Business.Lancamentos.Dtos;
using MoneyLoris.Application.Common.Base;
using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Stubs;
public class FaturaServiceStub : ServiceBase, IFaturaService
{  
    public Task<Result<FaturaInfoDto>> ObterInfo(FaturaFiltroDto filtro)
    {
        var info = new FaturaInfoDto
        {
            Mes = 6,
            Ano = 2023,
            DataFechamento = new DateTime(2023, 7, 3),
            DataVencimento = new DateTime(2023, 7, 1),
            ValorFatura = -4318.00M, //garantir que sempre volte negativo
            ValorPago = 4318.00M, //garantir que sempre volte positivo
        };

        return TaskSuccess(info);
    }

    public Task<Result<Pagination<ICollection<LancamentoListItemDto>>>> Pesquisar(FaturaFiltroDto filtro)
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
        var list = new List<LancamentoListItemDto>
        {
            new LancamentoListItemDto
            {
                Id = 501,
                Data = SystemTime.Today(),
                Tipo = TipoLancamento.Despesa,
                Operacao = OperacaoLancamento.LancamentoSimples,
                MeioPagamentoNome = "Nubank",
                MeioPagamentoTipo = TipoMeioPagamento.CartaoCredito,
                MeioPagamentoCor = "820AD1",
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
                Id = 505,
                Data = SystemTime.Today().AddDays(-3),
                Tipo = TipoLancamento.Despesa,
                Operacao = OperacaoLancamento.LancamentoSimples,
                MeioPagamentoNome = "Nubank",
                MeioPagamentoTipo = TipoMeioPagamento.CartaoCredito,
                MeioPagamentoCor = "820AD1",
                Categoria = "Assinaturas",
                Subcategoria = null!,
                Descricao = "Recarga Celular",
                Valor = -50.00M
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

        };

        for (int i = 1; i <= 50; i++)
        {
            list.Add(new LancamentoListItemDto
            {
                Id = 600 + i,
                Data = DateTime.Now.AddDays(-(10 + i)),
                Tipo = TipoLancamento.Despesa,
                Operacao = OperacaoLancamento.LancamentoSimples,
                MeioPagamentoNome = "Nubank",
                MeioPagamentoTipo = TipoMeioPagamento.CartaoCredito,
                MeioPagamentoCor = "820AD1",
                Categoria = "Outras Despesas",
                Subcategoria = null!,
                Descricao = $"Compra {i}",
                Valor = -(25.00M + i)
            });
        }

        return list;
    }


    public Task<Result<FaturaSelecaoDto>> ObterFaturaAtual(int IdCartao)
    {
        var fat = new FaturaSelecaoDto {
            Mes = DateTime.Today.Month,
            Ano = DateTime.Today.Year
        };

        return TaskSuccess(fat);
    }

    public Task<Result<ICollection<FaturaSelecaoDto>>> ObterFaturasSelecao(int IdCartao)
    {
        ICollection<FaturaSelecaoDto> list = new List<FaturaSelecaoDto>();

        var d = DateTime.Today.AddMonths(-1);

        for (int i = 0; i < 12; i++)
        {
            var f = new FaturaSelecaoDto
            {
                Mes = d.Month,
                Ano = d.Year
            };

            list.Add(f);

            d = d.AddMonths(1);
        }

        return TaskSuccess(list);
    }

    public async Task<Fatura> ObterOuCriarFatura(MeioPagamento cartao, int mes, int ano)
    {
        var fatura = new Fatura
        {
            Id = 123,
            IdMeioPagamento = cartao.Id,
            Mes = mes,
            Ano = ano,
            DataInicio = new DateTime(2023, 5, 3),
            DataFim = new DateTime(2023, 6, 2),
            DataVencimento = new DateTime(2023, 6, 10),
            ValorPago = null
        };

        return fatura;
    }
}
