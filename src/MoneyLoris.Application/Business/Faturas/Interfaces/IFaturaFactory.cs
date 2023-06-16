using MoneyLoris.Application.Domain.Entities;

namespace MoneyLoris.Application.Business.Faturas.Interfaces;
public interface IFaturaFactory
{
    Fatura Criar(MeioPagamento cartao, int mes, int ano);
}
