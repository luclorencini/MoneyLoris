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
                l.Operacao == OperacaoLancamento.LancamentoSimples &&
                l.Tipo == TipoLancamento.Despesa
            )
            .SumAsync(l => l.Valor);

        return total;
    }

    public async Task<ICollection<Lancamento>> PesquisaPaginada(FaturaFiltroDto filtro, int idUsuario)
    {
        //nota sobre o order by:
        //queremos agrupar todos os lançamentos que são parcelamentos de compras feitas em meses anteriores no começo da listagem, simulando como uma fatura de banco faz.
        //já o lançamento de compra parcelada no mês atual (parcelaAtual = 1), queremos que apareça ordenado normalmente por data junto com as outras compras à vista

        var query = _context.Lancamentos
            .Include(l => l.MeioPagamento)
            .Include(l => l.Categoria)
            .Include(l => l.Subcategoria)
            .Include(l => l.Fatura)
            .Where(whereQueryListagem(filtro, idUsuario))
            .OrderBy(l => l.ParcelaAtual == null || l.ParcelaAtual == 1) //agrupar parcelamentos antigos
            .ThenBy(l => l.Data)
            .ThenBy(l => l.Id);

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
