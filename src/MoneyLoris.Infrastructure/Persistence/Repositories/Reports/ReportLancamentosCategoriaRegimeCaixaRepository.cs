using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Reports.LancamentosCategoria;
using MoneyLoris.Application.Reports.LancamentosCategoria.Dto;
using MoneyLoris.Application.Utils;
using MoneyLoris.Infrastructure.Persistence.Context;

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
    {SubqueriesCategoriaComSubcategoriaFaturas(filtro)}	
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
    {SubqueriesCategoriaSemSubcategoriaFaturas(filtro)}
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

    private string SubqueriesCategoriaComSubcategoriaFaturas(ReportLancamentoFilterDto filtro)
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

    private string SubqueriesCategoriaSemSubcategoriaFaturas(ReportLancamentoFilterDto filtro)
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

    public Task<int> DetalheTotalRegistros(int idUsuario, ReportLancamentoDetalheFilterDto filtro)
    {
        throw new NotImplementedException();
    }

    public Task<ICollection<Lancamento>> DetalhePaginado(int idUsuario, ReportLancamentoDetalheFilterDto filtro)
    {
        throw new NotImplementedException();
    }

    public Task<decimal> DetalheSomatorio(int idUsuario, ReportLancamentoDetalheFilterDto filtro)
    {
        throw new NotImplementedException();
    }

    #endregion
}
