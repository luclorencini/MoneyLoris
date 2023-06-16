using MoneyLoris.Application.Domain.Entities;

namespace MoneyLoris.Application.Business.Faturas.Interfaces;
public interface IFaturaValidator
{
    void Existe(Fatura fatura);
}
