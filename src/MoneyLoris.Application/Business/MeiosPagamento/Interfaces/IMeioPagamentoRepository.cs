using MoneyLoris.Application.Common.Interfaces;
using MoneyLoris.Application.Domain.Entities;

namespace MoneyLoris.Application.Business.MeiosPagamento.Interfaces;
public interface IMeioPagamentoRepository : IRepositoryBase<MeioPagamento>
{
    Task<ICollection<MeioPagamento>> ListarMeiosPagamentoUsuario(int idUsuario);
    Task<ICollection<MeioPagamento>> ListarContasUsuario(int idUsuario);
    Task<ICollection<MeioPagamento>> ListarCartoesUsuario(int idUsuario);

    Task<ICollection<(int id, decimal saldo)>> CalcularSaldoAtualMeiosPagamentoUsuario(int idUsuario);
}
