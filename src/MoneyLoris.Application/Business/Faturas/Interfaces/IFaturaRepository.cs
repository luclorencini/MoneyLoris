using MoneyLoris.Application.Common.Interfaces;
using MoneyLoris.Application.Domain.Entities;

namespace MoneyLoris.Application.Business.Faturas.Interfaces;
public interface IFaturaRepository : IRepositoryBase<Fatura>
{
    Task<Fatura> ObterPorMesAno(int idCartao, int mes, int ano);
}
