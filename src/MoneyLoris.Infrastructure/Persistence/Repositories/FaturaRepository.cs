using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MoneyLoris.Application.Business.Faturas.Dtos;
using MoneyLoris.Application.Business.Faturas.Interfaces;
using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Infrastructure.Persistence.Context;
using MoneyLoris.Infrastructure.Persistence.Repositories.Base;

namespace MoneyLoris.Infrastructure.Persistence.Repositories;
public class FaturaRepository : RepositoryBase<Fatura>, IFaturaRepository
{
    public FaturaRepository(BaseApplicationDbContext context) : base(context) { }

    public async Task<Fatura> ObterPorMesAno(int idCartao, int mes, int ano)
    {
        var ent = await _dbset
            .Where(
                f => f.IdMeioPagamento == idCartao &&
                f.Ano == ano &&
                f.Mes == mes
            )
            .SingleOrDefaultAsync();

        return ent!;
    }

    public async Task<decimal> ObterValorFatura(int idFatura, int idUsuario)
    {
        var total = await _context.Lancamentos
            .Where(
                l => l.IdUsuario == idUsuario &&
                l.IdFatura == idFatura &&
                l.Tipo == Application.Domain.Enums.TipoLancamento.Despesa
            )
            .SumAsync(l => l.Valor);

        return total;
    }

    public async Task<ICollection<Lancamento>> PesquisaPaginada(FaturaFiltroDto filtro, int idUsuario)
    {
        var query = _context.Lancamentos
            .Include(l => l.MeioPagamento)
            .Include(l => l.Categoria)
            .Include(l => l.Subcategoria)
            .Include(l => l.Fatura)
            .Where(whereQueryListagem(filtro, idUsuario))
            .OrderByDescending(l => l.Data)
            .ThenByDescending(l => l.Id);

        var list = await query
            .IncluiPaginacao(filtro)
            .AsNoTracking()
            .ToListAsync();

        return list;
    }

    public async Task<int> PesquisaTotalRegistros(FaturaFiltroDto filtro, int idUsuario)
    {
        var total = await _context.Lancamentos
            .Where(whereQueryListagem(filtro, idUsuario))
            .AsNoTracking()
            .CountAsync();

        return total;
    }

    private Expression<Func<Lancamento, bool>> whereQueryListagem(FaturaFiltroDto filtro, int idUsuario)
    {
        Expression<Func<Lancamento, bool>> query =
            l => l.IdUsuario == idUsuario
            && l.IdFatura == filtro.IdFatura
            && l.Operacao == OperacaoLancamento.LancamentoSimples
            ;

        return query;
    }
}
