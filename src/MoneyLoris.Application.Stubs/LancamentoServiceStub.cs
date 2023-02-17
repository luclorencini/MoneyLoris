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

    private ICollection<LancamentoListItemDto> MockList()
    {
        var list = new List<LancamentoListItemDto>
        {
            new LancamentoListItemDto
            {
                Id = 501,
                Data = new DateTime(2023, 1, 27),
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
                Data = new DateTime(2023, 1, 27),
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
                Data = new DateTime(2023, 1, 27),
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
                Data = new DateTime(2023, 1, 26),
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
                Data = new DateTime(2023, 1, 25),
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
                Data = new DateTime(2023, 1, 15),
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
                Data = new DateTime(2023, 1, 15),
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
                Data = new DateTime(2023, 1, 10),
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
                Data = new DateTime(2023, 1, 10),
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
                Data = DateTime.Now.AddDays(-(50 + i)),
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
