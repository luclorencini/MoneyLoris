using MoneyLoris.Application.Business.Faturas.Interfaces;
using MoneyLoris.Application.Domain.Entities;

namespace MoneyLoris.Application.Business.Faturas;
public class FaturaFactory : IFaturaFactory
{
    public Fatura Criar(MeioPagamento cartao, int mes, int ano)
    {

        // o "Mês da Fatura" é o mês que a fatura vence.
        // data de vencimento é composto pelo: dia de vencimento / mes da fatura / ano da fatura
        // A data de fechamento da fatura é o dia em que se fecham as contas do mês anterior para emitir a fatura do cartão.
        //  Assim, a partir dessa data, começa o período de vigência da fatura do mês, que vai até um dia antes do fechamento da próxima fatura

        var dtIni = new DateTime(ano, mes, cartao.DiaFechamento!.Value).AddMonths(-1);
        var dtFim = dtIni.AddMonths(1).AddDays(-1);
        var dtVen = new DateTime(ano, mes, cartao.DiaVencimento!.Value);

        var fatura = new Fatura
        {
            IdMeioPagamento = cartao.Id,
            Mes = mes,
            Ano = ano,
            DataInicio = dtIni,
            DataFim = dtFim,
            DataVencimento = dtVen,
            ValorPago = null //fatura sempre cria sem pagamento
        };

        return fatura;
    }
}
