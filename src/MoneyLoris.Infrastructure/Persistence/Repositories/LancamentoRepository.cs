using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MoneyLoris.Application.Business.Lancamentos;
using MoneyLoris.Application.Business.Lancamentos.Dtos;
using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Infrastructure.Persistence.Context;
using MoneyLoris.Infrastructure.Persistence.Repositories.Base;

namespace MoneyLoris.Infrastructure.Persistence.Repositories;
public class LancamentoRepository : RepositoryBase<Lancamento>, ILancamentoRepository
{
    public LancamentoRepository(BaseApplicationDbContext context) : base(context) { }

    public async Task<ICollection<Lancamento>> PesquisaPaginada(LancamentoFiltroDto filtro, int idUsuario)
    {
        var list = await _dbset
            .Include(l => l.MeioPagamento)
            .Include(l => l.Categoria)
            .Include(l => l.Subcategoria)
            .Where(whereQueryListagem(filtro, idUsuario))
            .OrderByDescending(l => l.Data)
            .ThenByDescending(l => l.Id)
            .IncluiPaginacao(filtro)
            .AsNoTracking()
            .ToListAsync();

        return list;
    }

    public async Task<int> PesquisaTotalRegistros(LancamentoFiltroDto filtro, int idUsuario)
    {
        var total = await _dbset
            .Where(whereQueryListagem(filtro, idUsuario))
            .AsNoTracking()
            .CountAsync();

        return total;
    }

    public async Task<decimal> SomatorioReceitasFiltro(LancamentoFiltroDto filtro, int idUsuario)
    {
        var val = await _dbset
            .Where(whereQueryListagem(filtro, idUsuario))
            .Where(l => l.Tipo == TipoLancamento.Receita)
            .Where(l => l.Operacao == OperacaoLancamento.LancamentoSimples)
            .SumAsync(l => l.Valor);

            return val;
    }

    public async Task<decimal> SomatorioDespesasFiltro(LancamentoFiltroDto filtro, int idUsuario)
    {
        var val = await _dbset
            .Where(whereQueryListagem(filtro, idUsuario))
            .Where(l => l.Tipo == TipoLancamento.Despesa)
            .Where(l => l.Operacao == OperacaoLancamento.LancamentoSimples)
            .SumAsync(l => l.Valor);

        return val;
    }

    private Expression<Func<Lancamento, bool>> whereQueryListagem(LancamentoFiltroDto filtro, int idUsuario)
    {
        //TODO - aplicar o resto dos filtros

        Expression<Func<Lancamento, bool>> query =
            l => l.IdUsuario == idUsuario
            && (filtro.Descricao == null || l.Descricao.ToUpper().Contains(filtro.Descricao.ToUpper()))
            && (filtro.DataInicio == null || l.Data >= filtro.DataInicio)
            && (filtro.DataFim == null || l.Data <= filtro.DataFim)
            ;

        return query;
    }

    public async Task<ICollection<Lancamento>> ObterLancamentosRecentes(int idUsuario, TipoLancamento tipo, string termoBusca)
    {
        var list = await _dbset
            .Include(l => l.MeioPagamento)
            .Include(l => l.Categoria)
            .Include(l => l.Subcategoria)
            .Where(l => 
                l.IdUsuario == idUsuario &&
                l.Tipo == tipo &&
                l.Operacao == OperacaoLancamento.LancamentoSimples &&
                (termoBusca == null || l.Descricao.ToUpper().Contains(termoBusca.ToUpper())))
            .Take(20)
            .OrderByDescending(l => l.Data)
            .ThenByDescending(l => l.Id)
            .AsNoTracking()
            .ToListAsync();

        return list;
    }

}
