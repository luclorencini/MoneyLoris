using MoneyLoris.Application.Business.Faturas.Dtos;
using MoneyLoris.Application.Common.Interfaces;
using MoneyLoris.Application.Domain.Entities;

namespace MoneyLoris.Application.Business.Faturas.Interfaces;
public interface IFaturaRepository : IRepositoryBase<Fatura>
{
    Task<Fatura> ObterPorMesAno(int idCartao, int mes, int ano);
    Task<decimal> ObterValorFatura(int idFatura, int idUsuario);
    Task<ICollection<Lancamento>> PesquisaPaginada(FaturaFiltroDto filtro, int idUsuario);
    Task<int> PesquisaTotalRegistros(FaturaFiltroDto filtro, int idUsuario);
}
