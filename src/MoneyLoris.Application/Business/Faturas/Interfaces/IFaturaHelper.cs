using MoneyLoris.Application.Domain.Entities;

namespace MoneyLoris.Application.Business.Faturas.Interfaces;
public interface IFaturaHelper
{
    Task<Fatura> ObterOuCriarFatura(MeioPagamento cartao, int mes, int ano);
    Task LancarValorPagoFatura(Fatura fatura, decimal valorInformado);
}
