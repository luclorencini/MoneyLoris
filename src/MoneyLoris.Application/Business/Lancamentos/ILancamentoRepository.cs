using MoneyLoris.Application.Business.Lancamentos.Dtos;
using MoneyLoris.Application.Common.Interfaces;
using MoneyLoris.Application.Domain.Entities;

namespace MoneyLoris.Application.Business.Lancamentos;
public interface ILancamentoRepository : IRepositoryBase<Lancamento>
{
    Task<ICollection<Lancamento>> PesquisaPaginada(LancamentoFiltroDto filtro, int idUsuario);
    Task<int> PesquisaTotalRegistros(LancamentoFiltroDto filtro, int idUsuario);
    Task<decimal> SomatorioReceitasFiltro(LancamentoFiltroDto filtro, int idUsuario);
    Task<decimal> SomatorioDespesasFiltro(LancamentoFiltroDto filtro, int idUsuario);
}
