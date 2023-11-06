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

    public async Task<Lancamento> ObterInfo(int id)
    {
        var ent = await _dbset
            .Include(l => l.MeioPagamento)
            .Include(l => l.Fatura)
            .Where(l => l.Id == id)
            .SingleOrDefaultAsync();

        return ent!;
    }

    public async Task<ICollection<Lancamento>> PesquisaPaginada(LancamentoFiltroDto filtro, int idUsuario)
    {
        //nota sobre o order by:
        //queremos agrupar todos os lançamentos que são parcelamentos de compras feitas em meses anteriores no começo ou no final da listagem, dependendo da opção 'MaisAntigosPrimeiro'.
        //já o lançamento de compra parcelada no mês atual (parcelaAtual = 1), queremos que apareça ordenado normalmente por data junto com as outras compras à vista

        var query = _dbset
            .Include(l => l.MeioPagamento)
            .Include(l => l.Categoria)
            .Include(l => l.Subcategoria)
            .Include(l => l.Fatura)
            .Where(whereQueryListagem(filtro, idUsuario));


        if (filtro.MaisAntigosPrimeiro)
        {
            query = query
                .OrderBy(l => l.ParcelaAtual == null || l.ParcelaAtual == 1) //agrupar parcelamentos antigos
                .ThenBy(l => l.Data)
                .ThenBy(l => l.Id);
        }
        else
        {
            query = query
                .OrderBy(l => l.ParcelaAtual != null && l.ParcelaAtual > 1) //agrupar parcelamentos antigos
                .ThenByDescending(l => l.Data)
                .ThenByDescending(l => l.Id);
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
