using System.Data;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MoneyLoris.Application.Business.Lancamentos.Dtos;
using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Reports.LancamentosCategoria;
using MoneyLoris.Application.Reports.LancamentosCategoria.Dto;
using MoneyLoris.Application.Utils;
using MoneyLoris.Infrastructure.Persistence.Context;
using MoneyLoris.Infrastructure.Persistence.Repositories.Base;

namespace MoneyLoris.Infrastructure.Persistence.Repositories;
public class ReportLancamentosCategoriaRepository : IReportLancamentosCategoriaRepository
{
    private readonly BaseApplicationDbContext _context;

    public ReportLancamentosCategoriaRepository(BaseApplicationDbContext context)
    {
        _context = context;
    }

    #region Consolidado

    public List<CategoriaQueryResultItemtoDto> RelatorioLancamentosPorCategoria(int idUsuario, TipoLancamento tipo, int mes, int ano, int quantidade)
    {
        var t = Convert.ToInt32(tipo);

        var query = @$"
select c.Id as catId, c.Nome as catNome, c.Ordem as catOrdem, s.Id as subId, s.Nome as subNome, s.Ordem as subOrdem,
{GeraSubqueriesLancamentosComSubcategoria(mes, ano, quantidade)}
from categoria c
inner join subcategoria s on s.IdCategoria = c.Id
where c.Tipo = {t} and c.IdUsuario = {idUsuario}
UNION
select c.Id as catId, c.Nome as catNome, c.Ordem as catOrdem, NULL as subId, NULL as subNome, NULL as subOrdem, 
{GeraSubqueriesLancamentosSemSubcategoria(mes, ano, quantidade)}
from categoria c
where c.Tipo = {t} and c.IdUsuario = {idUsuario}
order by catOrdem is null, catOrdem, catNome, subNome is null, subOrdem is null, subOrdem
";


        var conn = _context.Database.GetDbConnection();

        using (var command = _context.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = query;
            command.CommandType = CommandType.Text;
            _context.Database.OpenConnection();

            var list = new List<CategoriaQueryResultItemtoDto>();
            using (var result = command.ExecuteReader())
            {
                while (result.Read())
                {
                    var obj = new CategoriaQueryResultItemtoDto();

                    obj.catId = Convert.IsDBNull(result["catId"]) ? null : (int?)result["catId"];
                    obj.catNome = Convert.IsDBNull(result["catNome"]) ? null : (string?)result["catNome"];
                    obj.catOrdem = Convert.IsDBNull(result["catOrdem"]) ? null : (sbyte?)result["catOrdem"];

                    obj.subId = Convert.IsDBNull(result["subId"]) ? null : (int?)result["subId"];
                    obj.subNome = Convert.IsDBNull(result["subNome"]) ? null : (string?)result["subNome"];
                    obj.subOrdem = Convert.IsDBNull(result["subOrdem"]) ? null : (sbyte?)result["subOrdem"];

                    if (quantidade >= 01) obj.val01 = Convert.IsDBNull(result["val01"]) ? null : (decimal?)result["val01"];
                    if (quantidade >= 02) obj.val02 = Convert.IsDBNull(result["val02"]) ? null : (decimal?)result["val02"];
                    if (quantidade >= 03) obj.val03 = Convert.IsDBNull(result["val03"]) ? null : (decimal?)result["val03"];
                    if (quantidade >= 04) obj.val04 = Convert.IsDBNull(result["val04"]) ? null : (decimal?)result["val04"];
                    if (quantidade >= 05) obj.val05 = Convert.IsDBNull(result["val05"]) ? null : (decimal?)result["val05"];
                    if (quantidade >= 06) obj.val06 = Convert.IsDBNull(result["val06"]) ? null : (decimal?)result["val06"];
                    if (quantidade >= 07) obj.val07 = Convert.IsDBNull(result["val07"]) ? null : (decimal?)result["val07"];
                    if (quantidade >= 08) obj.val08 = Convert.IsDBNull(result["val08"]) ? null : (decimal?)result["val08"];
                    if (quantidade >= 09) obj.val09 = Convert.IsDBNull(result["val09"]) ? null : (decimal?)result["val09"];
                    if (quantidade >= 10) obj.val10 = Convert.IsDBNull(result["val10"]) ? null : (decimal?)result["val10"];
                    if (quantidade >= 11) obj.val11 = Convert.IsDBNull(result["val11"]) ? null : (decimal?)result["val11"];
                    if (quantidade >= 12) obj.val12 = Convert.IsDBNull(result["val12"]) ? null : (decimal?)result["val12"];

                    list.Add(obj);
                }
            }
            _context.Database.CloseConnection();
            return list;
        }
    }


    private string GeraSubqueriesLancamentosComSubcategoria(int mes, int ano, int quantidade)
    {
        var ret = "";
        var dataInicial = new DateTime(ano, mes, 1);
        for (int i = 1; i <= quantidade; i++)
        {
            ret += $"(select sum(l.Valor) from lancamento l where (l.IdCategoria = c.Id and l.IdSubcategoria = s.Id and l.Data >= '{dataInicial.ToSqlStringYMD()}' and l.Data <= '{dataInicial.UltimoDiaMes().ToSqlStringYMD()}')) as val{i.ToString("D2")}";

            if (i < quantidade)
                ret += ",";

            dataInicial = dataInicial.AddMonths(1);
        }

        return ret;
    }

    private string GeraSubqueriesLancamentosSemSubcategoria(int mes, int ano, int quantidade)
    {
        var ret = "";
        var dataInicial = new DateTime(ano, mes, 1);
        for (int i = 1; i <= quantidade; i++)
        {
            ret += $"(select sum(l.Valor) from lancamento l where (l.IdCategoria = c.Id and l.IdSubcategoria is null and l.Data >= '{dataInicial.ToSqlStringYMD()}' and l.Data <= '{dataInicial.UltimoDiaMes().ToSqlStringYMD()}')) as val{i.ToString("D2")}";

            if (i < quantidade)
                ret += ",";

            dataInicial = dataInicial.AddMonths(1);
        }

        return ret;
    }

    #endregion

    #region Detalhe

    public async Task<int> DetalheTotalRegistros(ReportLancamentoDetalheFilterDto filtro, int idUsuario)
    {
        var total = await _context.Lancamentos
            .Where(whereQueryDetalhe(filtro, idUsuario))
            .AsNoTracking()
            .CountAsync();

        return total;
    }

    public async Task<ICollection<Lancamento>> DetalhePaginado(ReportLancamentoDetalheFilterDto filtro, int idUsuario)
    {
        var list = await _context.Lancamentos
            .Include(l => l.MeioPagamento)
            .Include(l => l.Categoria)
            .Include(l => l.Subcategoria)
            .Include(l => l.Fatura)
            .Where(whereQueryDetalhe(filtro, idUsuario))
            .OrderBy(l => l.Data).ThenBy(l => l.Id)
            .IncluiPaginacao(filtro)
            .AsNoTracking()
            .ToListAsync();

        return list;
    }

    public async Task<decimal> DetalheSomatorio(ReportLancamentoDetalheFilterDto filtro, int idUsuario)
    {
        var val = await _context.Lancamentos
            .Where(whereQueryDetalhe(filtro, idUsuario))
            .SumAsync(l => l.Valor);

        return val;
    }

    private Expression<Func<Lancamento, bool>> whereQueryDetalhe(ReportLancamentoDetalheFilterDto filtro, int idUsuario)
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
            && l.IdCategoria == filtro.IdCategoria;
        }

        else if (filtro.IdSubcategoria == null)
        {
            //traz todos os lançamentos da categoria informada que não possuem subcategoria informada

            query =
            l => l.IdUsuario == idUsuario
            && l.Data >= dataIni
            && l.Data <= dataFim
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
            && l.IdCategoria == filtro.IdCategoria
            && l.IdSubcategoria == filtro.IdSubcategoria;
        }

        return query;
    }

    #endregion
}