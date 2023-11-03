using MoneyLoris.Application.Business.Faturas.Interfaces;
using MoneyLoris.Application.Business.MeiosPagamento.Interfaces;
using MoneyLoris.Application.Domain.Entities;

namespace MoneyLoris.Application.Business.Faturas;
public class FaturaHelper : IFaturaHelper
{
    private readonly IFaturaFactory _faturaFactory;
    private readonly IFaturaRepository _faturaRepo;
    private readonly IMeioPagamentoValidator _meioPagamentoValidator;

    public FaturaHelper(
        IFaturaFactory faturaFactory,
        IFaturaRepository faturaRepo,
        IMeioPagamentoValidator meioPagamentoValidator
    )
    {
        _faturaFactory = faturaFactory;
        _faturaRepo = faturaRepo;
        _meioPagamentoValidator = meioPagamentoValidator;
    }
    public async Task<Fatura> ObterOuCriarFatura(MeioPagamento cartao, int mes, int ano)
    {
        _meioPagamentoValidator.EhCartaoCredito(cartao);

        //busca fatura pelos campos informados

        var fatura = await _faturaRepo.ObterPorMesAno(cartao.Id, mes, ano);

        if (fatura != null)
            return fatura;

        //se nao encontrou, cria uma nova fatura

        var novaFatura = _faturaFactory.Criar(cartao, mes, ano);

        await _faturaRepo.Insert(novaFatura);

        return novaFatura;

    }

    public async Task LancarValorPagoFatura(Fatura fatura, decimal valorInformado)
    {
        //fail-safe: se valor pago ainda não existe, seta zero para funcionar a soma
        if (fatura.ValorPago is null)
            fatura.ValorPago = 0;

        //incrementa o valor pago (importante caso já haja algum pagamento anterior desta mesma fatura)
        fatura.ValorPago = fatura.ValorPago + valorInformado;

        await _faturaRepo.Update(fatura);

        return;
    }
}
