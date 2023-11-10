using System.Data;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Reports.LancamentosCategoria;
using MoneyLoris.Application.Reports.LancamentosCategoria.Dto;
using MoneyLoris.Application.Utils;
using MoneyLoris.Infrastructure.Persistence.Context;
using MoneyLoris.Infrastructure.Persistence.Repositories.Base;

namespace MoneyLoris.Infrastructure.Persistence.Repositories.Reports;
public class ReportLancamentosCategoriaRegimeCaixaRepository : ReportLancamentosCategoriaBaseRepository, IReportLancamentosCategoriaRegimeCaixaRepository
{
    public ReportLancamentosCategoriaRegimeCaixaRepository(BaseApplicationDbContext context) : base(context)
    {
    }

    #region Consolidado

    public List<CategoriaQueryResultItemtoDto> RelatorioLancamentosPorCategoria(int idUsuario, TipoLancamento tipo, ReportLancamentoFilterDto filtro)
    {
        var t = Convert.ToInt32(tipo);

        var query = @$"
select catId, catNome, catOrdem, subId, subNome, subOrdem, 
{SumsValores(filtro)}
from 
(
	-- cat com sub - contas do mes
	select c.Id as catId, c.Nome as catNome, c.Ordem as catOrdem, s.Id as subId, s.Nome as subNome, s.Ordem as subOrdem,	
	{SubqueriesCategoriaComSubcategoriaContas(filtro)}
    from categoria c
	inner join subcategoria s on s.IdCategoria = c.Id
	where c.Tipo = {t} and c.IdUsuario = {idUsuario}
	union
	-- cat com sub - faturas do mes
	select c.Id as catId, c.Nome as catNome, c.Ordem as catOrdem, s.Id as subId, s.Nome as subNome, s.Ordem as subOrdem,
    {SubqueriesCategoriaComSubcategoriaCartoes(filtro)}	
	from categoria c
	inner join subcategoria s on s.IdCategoria = c.Id
	where c.Tipo = {t} and c.IdUsuario = {idUsuario}
	union
	-- cat sem sub - contas do mes
	select c.Id as catId, c.Nome as catNome, c.Ordem as catOrdem, NULL as subId, NULL as subNome, NULL as subOrdem, 
    {SubqueriesCategoriaSemSubcategoriaContas(filtro)}	
	from categoria c
	where c.Tipo = {t} and c.IdUsuario = {idUsuario}
	union
	-- cat sem sub - faturas do mes
	select c.Id as catId, c.Nome as catNome, c.Ordem as catOrdem, NULL as subId, NULL as subNome, NULL as subOrdem, 
    {SubqueriesCategoriaSemSubcategoriaCartoes(filtro)}
	from categoria c
	where c.Tipo = {t} and c.IdUsuario = {idUsuario}
	) x
group by catId, subId
order by catOrdem is null, catOrdem, catNome, subNome is null, subOrdem is null, subOrdem, subNome
";


        var list = ExecutaQueryConsolidado(query, filtro);
        return list;
    }

    private string SumsValores(ReportLancamentoFilterDto filtro)
    {
        var ret = "";
        for (int i = 1; i <= filtro.Quantidade; i++)
        {
            ret += @$" sum(val{i.ToString("D2")}) as val{i.ToString("D2")}";

            if (i < filtro.Quantidade)
                ret += ",";
        }

        return ret;
    }

    private string SubqueriesCategoriaComSubcategoriaContas(ReportLancamentoFilterDto filtro)
    {
        var ret = "";
        var data = new DateTime(filtro.Ano, filtro.Mes, 1);
        for (int i = 1; i <= filtro.Quantidade; i++)
        {
            ret += @$"(select sum(l.Valor) from lancamento l
                        inner join meiopagamento m on m.id = l.IdMeioPagamento
                        where m.Tipo != 3 and
                        (l.IdCategoria = c.Id and l.IdSubcategoria = s.Id and l.Data >= '{data.ToSqlStringYMD()}' and l.Data <= '{data.UltimoDiaMes().ToSqlStringYMD()}')) 
                       as val{i.ToString("D2")}";

            if (i < filtro.Quantidade)
                ret += ",";

            data = data.AddMonths(1);
        }

        return ret;
    }

    private string SubqueriesCategoriaComSubcategoriaCartoes(ReportLancamentoFilterDto filtro)
    {
        var ret = "";
        var data = new DateTime(filtro.Ano, filtro.Mes, 1);
        for (int i = 1; i <= filtro.Quantidade; i++)
        {
            ret += @$"(select sum(l.Valor) from lancamento l 
                        inner join meiopagamento m on m.id = l.IdMeioPagamento
                        inner join fatura f on f.id = l.IdFatura 
                        where m.Tipo = 3 and
                        (l.IdCategoria = c.Id and l.IdSubcategoria = s.Id and f.DataVencimento >= '{data.ToSqlStringYMD()}' and f.DataVencimento <= '{data.UltimoDiaMes().ToSqlStringYMD()}'))
                       as val{i.ToString("D2")}";

            if (i < filtro.Quantidade)
                ret += ",";

            data = data.AddMonths(1);
        }

        return ret;
    }

    private string SubqueriesCategoriaSemSubcategoriaContas(ReportLancamentoFilterDto filtro)
    {
        var ret = "";
        var data = new DateTime(filtro.Ano, filtro.Mes, 1);
        for (int i = 1; i <= filtro.Quantidade; i++)
        {
            ret += @$"(select sum(l.Valor) from lancamento l 
                        inner join meiopagamento m on m.id = l.IdMeioPagamento 
                        where m.Tipo != 3 and
                        (l.IdCategoria = c.Id and l.IdSubcategoria is null and l.Data >= '{data.ToSqlStringYMD()}' and l.Data <= '{data.UltimoDiaMes().ToSqlStringYMD()}')) 
                       as val{i.ToString("D2")}";

            if (i < filtro.Quantidade)
                ret += ",";

            data = data.AddMonths(1);
        }

        return ret;
    }

    private string SubqueriesCategoriaSemSubcategoriaCartoes(ReportLancamentoFilterDto filtro)
    {
        var ret = "";
        var data = new DateTime(filtro.Ano, filtro.Mes, 1);
        for (int i = 1; i <= filtro.Quantidade; i++)
        {
            ret += @$"(select sum(l.Valor) from lancamento l
                        inner join meiopagamento m on m.id = l.IdMeioPagamento
                        inner join fatura f on f.id = l.IdFatura
                        where m.Tipo = 3 and
                        (l.IdCategoria = c.Id and l.IdSubcategoria is null and f.DataVencimento >= '{data.ToSqlStringYMD()}' and f.DataVencimento <= '{data.UltimoDiaMes().ToSqlStringYMD()}'))
                       as val{i.ToString("D2")}";

            if (i < filtro.Quantidade)
                ret += ",";

            data = data.AddMonths(1);
        }

        return ret;
    }

    #endregion

    #region Detalhe

    public async Task<int> DetalheTotalRegistros(int idUsuario, ReportLancamentoDetalheFilterDto filtro)
    {
        var total = await 
            (
                _context.Lancamentos
                .Include(l => l.Fatura)
                .Where(whereQueryDetalheContas(idUsuario, filtro))
            )
            .Union
            (
                _context.Lancamentos
                .Include(l => l.Fatura)
                .Where(whereQueryDetalheCartoes(idUsuario, filtro))
            )
            .AsNoTracking()
            .CountAsync();

        return total;
    }

    public async Task<ICollection<Lancamento>> DetalhePaginado(int idUsuario, ReportLancamentoDetalheFilterDto filtro)
    {
        var list = await 
            (
                _context.Lancamentos
                .Include(l => l.MeioPagamento)
                .Include(l => l.Categoria)
                .Include(l => l.Subcategoria)
                .Include(l => l.Fatura)
                .Where(whereQueryDetalheContas(idUsuario, filtro))
            )
            .Union
            (
                _context.Lancamentos
                .Include(l => l.MeioPagamento)
                .Include(l => l.Categoria)
                .Include(l => l.Subcategoria)
                .Include(l => l.Fatura)
                .Where(whereQueryDetalheCartoes(idUsuario, filtro))
            )
            .OrderBy(l => l.Data).ThenBy(l => l.Id)
            .IncluiPaginacao(filtro)
            .AsNoTracking()
            .ToListAsync();

        return list;
    }

    public async Task<decimal> DetalheSomatorio(int idUsuario, ReportLancamentoDetalheFilterDto filtro)
    {
        var val = await
            (
                _context.Lancamentos
                .Include(l => l.MeioPagamento)
                .Include(l => l.Fatura)
                .Where(whereQueryDetalheContas(idUsuario, filtro))
            )
            .Union(
                _context.Lancamentos
                .Include(l => l.MeioPagamento)
                .Include(l => l.Fatura)
                .Where(whereQueryDetalheCartoes(idUsuario, filtro))
            )
            .SumAsync(l => l.Valor);

        return val;
    }

    private Expression<Func<Lancamento, bool>> whereQueryDetalheContas(int idUsuario, ReportLancamentoDetalheFilterDto filtro)
    {
        var dataIni = new DateTime(filtro.Ano, filtro.Mes, 1);
        var dataFim = dataIni.UltimoDiaMes();

        Expression<Func<Lancamento, bool>> query = null!;

        if (filtro.TodosDaCategoria)
        {
            //traz todos os lançamentos da categoria informada, independente se tem subcategoria ou não

            query =
            l => l.IdUsuario == idUsuario
            && l.Data >= dataIni
            && l.Data <= dataFim
            && l.MeioPagamento.Tipo != TipoMeioPagamento.CartaoCredito
            && l.IdCategoria == filtro.IdCategoria;
        }
        else if (filtro.IdSubcategoria == null)
        {
            //traz todos os lançamentos da categoria informada que não possuem subcategoria informada

            query =
            l => l.IdUsuario == idUsuario
            && l.Data >= dataIni
            && l.Data <= dataFim
            && l.MeioPagamento.Tipo != TipoMeioPagamento.CartaoCredito
            && l.IdCategoria == filtro.IdCategoria
            && l.IdSubcategoria == null;
        }
        else
        {
            //traz todos os lançamentos da categoria e subcategoria informadas

            query =
            l => l.IdUsuario == idUsuario
            && l.Data >= dataIni
            && l.Data <= dataFim
            && l.MeioPagamento.Tipo != TipoMeioPagamento.CartaoCredito
            && l.IdCategoria == filtro.IdCategoria
            && l.IdSubcategoria == filtro.IdSubcategoria;
        }

        return query;
    }

    private Expression<Func<Lancamento, bool>> whereQueryDetalheCartoes(int idUsuario, ReportLancamentoDetalheFilterDto filtro)
    {
        var dataIni = new DateTime(filtro.Ano, filtro.Mes, 1);
        var dataFim = dataIni.UltimoDiaMes();

        Expression<Func<Lancamento, bool>> query = null!;

        if (filtro.TodosDaCategoria)
        {
            //traz todos os lançamentos da categoria informada, independente se tem subcategoria ou não

            query =
            l => l.IdUsuario == idUsuario
            && l.Fatura.DataVencimento >= dataIni
            && l.Fatura.DataVencimento <= dataFim
            && l.MeioPagamento.Tipo == TipoMeioPagamento.CartaoCredito
            && l.IdCategoria == filtro.IdCategoria;
        }
        else if (filtro.IdSubcategoria == null)
        {
            //traz todos os lançamentos da categoria informada que não possuem subcategoria informada

            query =
            l => l.IdUsuario == idUsuario
            && l.Fatura.DataVencimento >= dataIni
            && l.Fatura.DataVencimento <= dataFim
            && l.MeioPagamento.Tipo == TipoMeioPagamento.CartaoCredito
            && l.IdCategoria == filtro.IdCategoria
            && l.IdSubcategoria == null;
        }
        else
        {
            //traz todos os lançamentos da categoria e subcategoria informadas

            query =
            l => l.IdUsuario == idUsuario
            && l.Fatura.DataVencimento >= dataIni
            && l.Fatura.DataVencimento <= dataFim
            && l.MeioPagamento.Tipo == TipoMeioPagamento.CartaoCredito
            && l.IdCategoria == filtro.IdCategoria
            && l.IdSubcategoria == filtro.IdSubcategoria;
        }

        return query;
    }

    #endregion
}
