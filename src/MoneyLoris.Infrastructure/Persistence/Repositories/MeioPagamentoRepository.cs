using Microsoft.EntityFrameworkCore;
using MoneyLoris.Application.Business.MeiosPagamento.Interfaces;
using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Infrastructure.Persistence.Context;
using MoneyLoris.Infrastructure.Persistence.Repositories.Base;

namespace MoneyLoris.Infrastructure.Persistence.Repositories;
public class MeioPagamentoRepository : RepositoryBase<MeioPagamento>, IMeioPagamentoRepository
{
    public MeioPagamentoRepository(BaseApplicationDbContext context) : base(context) { }

    public async Task<ICollection<MeioPagamento>> ListarMeiosPagamentoUsuario(int idUsuario)
    {
        var list = await _dbset
            .Where(m => m.IdUsuario == idUsuario)
            .OrderBy(m => m.Ordem == null)
            .ThenBy(m => m.Ordem)
            .ThenBy(m => m.Nome)
            .AsNoTracking()
            .ToListAsync();

        return list;
    }

    public async Task<ICollection<MeioPagamento>> ListarContasUsuario(int idUsuario)
    {
        var list = await _dbset
            .Where(m => m.IdUsuario == idUsuario && m.Tipo != TipoMeioPagamento.CartaoCredito)
            .OrderBy(m => m.Ordem == null)
            .ThenBy(m => m.Ordem)
            .ThenBy(m => m.Nome)
            .AsNoTracking()
            .ToListAsync();

        return list;
    }

    public async Task<ICollection<MeioPagamento>> ListarCartoesUsuario(int idUsuario)
    {
        var list = await _dbset
            .Where(m => m.IdUsuario == idUsuario && m.Tipo == TipoMeioPagamento.CartaoCredito)
            .OrderBy(m => m.Ordem == null)
            .ThenBy(m => m.Ordem)
            .ThenBy(m => m.Nome)
            .AsNoTracking()
            .ToListAsync();

        return list;
    }

    public async Task<ICollection<(int id, decimal saldo)>> CalcularSaldoAtualMeiosPagamentoUsuario(int idUsuario)
    {
        var dados = await _context.Lancamentos
            .Include(l => l.MeioPagamento)
            .Where(l => l.IdUsuario == idUsuario)
            .GroupBy(l => l.MeioPagamento.Id)
            .Select(l => new { id = l.Key, saldo = l.Sum(x => x.Valor) })
            .ToListAsync();

        var ret = dados.Select(x => (x.id, x.saldo)).ToList();

        return ret;
    }
}
