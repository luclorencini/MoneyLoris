using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MoneyLoris.Application.Business.Lancamentos.Dtos;
using MoneyLoris.Application.Business.Lancamentos.Interfaces;
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
        var query = _dbset
            .Include(l => l.MeioPagamento)
            .Include(l => l.Categoria)
            .Include(l => l.Subcategoria)
            .Where(whereQueryListagem(filtro, idUsuario));


        if (filtro.MaisAntigosPrimeiro)
        {
            query = query.OrderBy(l => l.Data).ThenBy(l => l.Id);
        }
        else
        {
            query = query.OrderByDescending(l => l.Data).ThenByDescending(l => l.Id);
        }

        var list = await query
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
        var val = await SomatorioFiltro(filtro, idUsuario, TipoLancamento.Receita);
        return val;
    }

    public async Task<decimal> SomatorioDespesasFiltro(LancamentoFiltroDto filtro, int idUsuario)
    {
        var val = await SomatorioFiltro(filtro, idUsuario, TipoLancamento.Despesa);
        return val;
    }

    private async Task<decimal> SomatorioFiltro(LancamentoFiltroDto filtro, int idUsuario, TipoLancamento tipo)
    {
        var val = await _dbset
            .Where(whereQueryListagem(filtro, idUsuario))
            .Where(l => l.Tipo == tipo)
            .Where(l => filtro.SomarTransferencias || l.Operacao == OperacaoLancamento.LancamentoSimples)
            .SumAsync(l => l.Valor);

        return val;
    }


    private Expression<Func<Lancamento, bool>> whereQueryListagem(LancamentoFiltroDto filtro, int idUsuario)
    {
        Expression<Func<Lancamento, bool>> query =
            l => l.IdUsuario == idUsuario
            && (filtro.Descricao == null || l.Descricao.ToUpper().Contains(filtro.Descricao.ToUpper()))
            && (filtro.DataInicio == null || l.Data >= filtro.DataInicio)
            && (filtro.DataFim == null || l.Data <= filtro.DataFim)
            && (filtro.IdMeioPagamento == null || l.IdMeioPagamento == filtro.IdMeioPagamento)
            && (filtro.IdCategoria == null || l.IdCategoria == filtro.IdCategoria)
            && (filtro.IdSubcategoria == null || l.IdSubcategoria == filtro.IdSubcategoria)

            && (
                (filtro.TrazerReceitas && filtro.TrazerDespesas) ||
                (filtro.TrazerReceitas && l.Tipo == TipoLancamento.Receita) ||
                (filtro.TrazerDespesas && l.Tipo == TipoLancamento.Despesa)
            )
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
                l.Data <= DateTime.Today &&
                (termoBusca == null || l.Descricao.ToUpper().Contains(termoBusca.ToUpper())))
            .OrderByDescending(l => l.Data)
            .ThenByDescending(l => l.Id)
            .Take(35)
            .AsNoTracking()
            .ToListAsync();

        return list;
    }

    public async Task<int> QuantidadePorMeioPagamento(int idMeioPagamento)
    {
        var qtde = await _dbset
            .Where(l => l.IdMeioPagamento == idMeioPagamento)
            .CountAsync();

        return qtde;
    }

}
