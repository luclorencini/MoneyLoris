using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
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

    public List<CategoriaQueryResultItemtoDto> RelatorioLancamentosPorCategoria(int idUsuario, TipoLancamento tipo, ReportLancamentoFilterDto filtro)
    {
        var t = Convert.ToInt32(tipo);

        var query = @$"
select c.Id as catId, c.Nome as catNome, c.Ordem as catOrdem, s.Id as subId, s.Nome as subNome, s.Ordem as subOrdem,
{GeraSubqueriesLancamentosComSubcategoria(filtro)}
from categoria c
inner join subcategoria s on s.IdCategoria = c.Id
where c.Tipo = {t} and c.IdUsuario = {idUsuario}
UNION
select c.Id as catId, c.Nome as catNome, c.Ordem as catOrdem, NULL as subId, NULL as subNome, NULL as subOrdem, 
{GeraSubqueriesLancamentosSemSubcategoria(filtro)}
from categoria c
where c.Tipo = {t} and c.IdUsuario = {idUsuario}
order by catOrdem is null, catOrdem, catNome, subNome is null, subOrdem is null, subOrdem
";


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
                    var dto = ConvertToDto(result, filtro);
                    list.Add(dto);
                }
            }
            _context.Database.CloseConnection();
            return list;
        }
    }

    private string GeraSubqueriesLancamentosComSubcategoria(ReportLancamentoFilterDto filtro)
    {
        var ret = "";
        var dataInicial = new DateTime(filtro.Ano, filtro.Mes, 1);
        for (int i = 1; i <= filtro.Quantidade; i++)
        {
            ret += $"(select sum(l.Valor) from lancamento l where (l.IdCategoria = c.Id and l.IdSubcategoria = s.Id and l.Data >= '{dataInicial.ToSqlStringYMD()}' and l.Data <= '{dataInicial.UltimoDiaMes().ToSqlStringYMD()}')) as val{i.ToString("D2")}";

            if (i < filtro.Quantidade)
                ret += ",";

            dataInicial = dataInicial.AddMonths(1);
        }

        return ret;
    }

    private string GeraSubqueriesLancamentosSemSubcategoria(ReportLancamentoFilterDto filtro)
    {
        var ret = "";
        var dataInicial = new DateTime(filtro.Ano, filtro.Mes, 1);
        for (int i = 1; i <= filtro.Quantidade; i++)
        {
            ret += $"(select sum(l.Valor) from lancamento l where (l.IdCategoria = c.Id and l.IdSubcategoria is null and l.Data >= '{dataInicial.ToSqlStringYMD()}' and l.Data <= '{dataInicial.UltimoDiaMes().ToSqlStringYMD()}')) as val{i.ToString("D2")}";

            if (i < filtro.Quantidade)
                ret += ",";

            dataInicial = dataInicial.AddMonths(1);
        }

        return ret;
    }

    private CategoriaQueryResultItemtoDto ConvertToDto(DbDataReader result, ReportLancamentoFilterDto filtro)
    {
        var dto = new CategoriaQueryResultItemtoDto();

        dto.catId = result.ReadColumn<int?>("catId");
        dto.catNome = result.ReadColumn<string?>("catNome");
        dto.catOrdem = result.ReadColumn<sbyte?>("catOrdem");

        dto.subId = result.ReadColumn<int?>("subId");
        dto.subNome = result.ReadColumn<string?>("subNome");
        dto.subOrdem = result.ReadColumn<sbyte?>("subOrdem");

        if (filtro.Quantidade >= 01) dto.val01 = result.ReadColumn<decimal?>("val01");
        if (filtro.Quantidade >= 02) dto.val02 = result.ReadColumn<decimal?>("val02");
        if (filtro.Quantidade >= 03) dto.val03 = result.ReadColumn<decimal?>("val03");
        if (filtro.Quantidade >= 04) dto.val04 = result.ReadColumn<decimal?>("val04");
        if (filtro.Quantidade >= 05) dto.val05 = result.ReadColumn<decimal?>("val05");
        if (filtro.Quantidade >= 06) dto.val06 = result.ReadColumn<decimal?>("val06");
        if (filtro.Quantidade >= 07) dto.val07 = result.ReadColumn<decimal?>("val07");
        if (filtro.Quantidade >= 08) dto.val08 = result.ReadColumn<decimal?>("val08");
        if (filtro.Quantidade >= 09) dto.val09 = result.ReadColumn<decimal?>("val09");
        if (filtro.Quantidade >= 10) dto.val10 = result.ReadColumn<decimal?>("val10");
        if (filtro.Quantidade >= 11) dto.val11 = result.ReadColumn<decimal?>("val11");
        if (filtro.Quantidade >= 12) dto.val12 = result.ReadColumn<decimal?>("val12");

        return dto;
    }

    #endregion

    #region Detalhe

    public async Task<int> DetalheTotalRegistros(int idUsuario, ReportLancamentoDetalheFilterDto filtro)
    {
        var total = await _context.Lancamentos
            .Where(whereQueryDetalhe(idUsuario, filtro))
            .AsNoTracking()
            .CountAsync();

        return total;
    }

    public async Task<ICollection<Lancamento>> DetalhePaginado(int idUsuario, ReportLancamentoDetalheFilterDto filtro)
    {
        var list = await _context.Lancamentos
            .Include(l => l.MeioPagamento)
            .Include(l => l.Categoria)
            .Include(l => l.Subcategoria)
            .Include(l => l.Fatura)
            .Where(whereQueryDetalhe(idUsuario, filtro))
            .OrderBy(l => l.Data).ThenBy(l => l.Id)
            .IncluiPaginacao(filtro)
            .AsNoTracking()
            .ToListAsync();

        return list;
    }

    public async Task<decimal> DetalheSomatorio(int idUsuario, ReportLancamentoDetalheFilterDto filtro)
    {
        var val = await _context.Lancamentos
            .Where(whereQueryDetalhe(idUsuario, filtro))
            .SumAsync(l => l.Valor);

        return val;
    }

    private Expression<Func<Lancamento, bool>> whereQueryDetalhe(int idUsuario, ReportLancamentoDetalheFilterDto filtro)
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