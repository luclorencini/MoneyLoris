using System.Data;
using Microsoft.EntityFrameworkCore;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Reports.LancamentosCategoria;
using MoneyLoris.Application.Reports.LancamentosCategoria.Dto;
using MoneyLoris.Infrastructure.Persistence.Context;

namespace MoneyLoris.Infrastructure.Persistence.Repositories;
public class ReportLancamentosCategoriaRepository : IReportLancamentosCategoriaRepository
{
    private readonly BaseApplicationDbContext _context;

    public ReportLancamentosCategoriaRepository(BaseApplicationDbContext context)
    {
        _context = context;
    }


    public List<CategoriaQueryResultItemtoDto> RelatorioLancamentosPorCategoria(int idUsuario, TipoLancamento tipo, int mes, int ano, int quantidade)
    {
        var t = Convert.ToInt32(tipo);

        //todo - query tá com datas fixas para todos os meses de 2023. Parametrizar no futuro

        var query = @$"
select c.Id as catId, c.Nome as catNome, c.Ordem as catOrdem, s.Id as subId, s.Nome as subNome, s.Ordem as subOrdem,
 (select sum(l.Valor) from lancamento l where (l.IdCategoria = c.Id and l.IdSubcategoria = s.Id and l.Data >= '2023-01-01' and l.Data <= '2023-01-31')) as val01,
 (select sum(l.Valor) from lancamento l where (l.IdCategoria = c.Id and l.IdSubcategoria = s.Id and l.Data >= '2023-02-01' and l.Data <= '2023-02-28')) as val02,
 (select sum(l.Valor) from lancamento l where (l.IdCategoria = c.Id and l.IdSubcategoria = s.Id and l.Data >= '2023-03-01' and l.Data <= '2023-03-31')) as val03,
 (select sum(l.Valor) from lancamento l where (l.IdCategoria = c.Id and l.IdSubcategoria = s.Id and l.Data >= '2023-04-01' and l.Data <= '2023-04-30')) as val04,
 (select sum(l.Valor) from lancamento l where (l.IdCategoria = c.Id and l.IdSubcategoria = s.Id and l.Data >= '2023-05-01' and l.Data <= '2023-05-31')) as val05,
 (select sum(l.Valor) from lancamento l where (l.IdCategoria = c.Id and l.IdSubcategoria = s.Id and l.Data >= '2023-06-01' and l.Data <= '2023-06-30')) as val06,
 (select sum(l.Valor) from lancamento l where (l.IdCategoria = c.Id and l.IdSubcategoria = s.Id and l.Data >= '2023-07-01' and l.Data <= '2023-07-31')) as val07,
 (select sum(l.Valor) from lancamento l where (l.IdCategoria = c.Id and l.IdSubcategoria = s.Id and l.Data >= '2023-08-01' and l.Data <= '2023-08-31')) as val08,
 (select sum(l.Valor) from lancamento l where (l.IdCategoria = c.Id and l.IdSubcategoria = s.Id and l.Data >= '2023-09-01' and l.Data <= '2023-09-30')) as val09,
 (select sum(l.Valor) from lancamento l where (l.IdCategoria = c.Id and l.IdSubcategoria = s.Id and l.Data >= '2023-10-01' and l.Data <= '2023-10-31')) as val10,
 (select sum(l.Valor) from lancamento l where (l.IdCategoria = c.Id and l.IdSubcategoria = s.Id and l.Data >= '2023-11-01' and l.Data <= '2023-11-30')) as val11,
 (select sum(l.Valor) from lancamento l where (l.IdCategoria = c.Id and l.IdSubcategoria = s.Id and l.Data >= '2023-12-01' and l.Data <= '2023-12-31')) as val12
from categoria c
inner join subcategoria s on s.IdCategoria = c.Id
where c.Tipo = {t} and c.IdUsuario = {idUsuario}
UNION
select c.Id as catId, c.Nome as catNome, c.Ordem as catOrdem, NULL as subId, NULL as subNome, NULL as subOrdem,
 (select sum(l.Valor) from lancamento l where (l.IdCategoria = c.Id and l.IdSubcategoria is null and l.Data >= '2023-01-01' and l.Data <= '2023-01-31')) as val01,
 (select sum(l.Valor) from lancamento l where (l.IdCategoria = c.Id and l.IdSubcategoria is null and l.Data >= '2023-02-01' and l.Data <= '2023-02-28')) as val02,
 (select sum(l.Valor) from lancamento l where (l.IdCategoria = c.Id and l.IdSubcategoria is null and l.Data >= '2023-03-01' and l.Data <= '2023-03-31')) as val03,
 (select sum(l.Valor) from lancamento l where (l.IdCategoria = c.Id and l.IdSubcategoria is null and l.Data >= '2023-04-01' and l.Data <= '2023-04-30')) as val04,
 (select sum(l.Valor) from lancamento l where (l.IdCategoria = c.Id and l.IdSubcategoria is null and l.Data >= '2023-05-01' and l.Data <= '2023-05-31')) as val05,
 (select sum(l.Valor) from lancamento l where (l.IdCategoria = c.Id and l.IdSubcategoria is null and l.Data >= '2023-06-01' and l.Data <= '2023-06-30')) as val06,
 (select sum(l.Valor) from lancamento l where (l.IdCategoria = c.Id and l.IdSubcategoria is null and l.Data >= '2023-07-01' and l.Data <= '2023-07-31')) as val07,
 (select sum(l.Valor) from lancamento l where (l.IdCategoria = c.Id and l.IdSubcategoria is null and l.Data >= '2023-08-01' and l.Data <= '2023-08-31')) as val08,
 (select sum(l.Valor) from lancamento l where (l.IdCategoria = c.Id and l.IdSubcategoria is null and l.Data >= '2023-09-01' and l.Data <= '2023-09-30')) as val09,
 (select sum(l.Valor) from lancamento l where (l.IdCategoria = c.Id and l.IdSubcategoria is null and l.Data >= '2023-10-01' and l.Data <= '2023-10-31')) as val10,
 (select sum(l.Valor) from lancamento l where (l.IdCategoria = c.Id and l.IdSubcategoria is null and l.Data >= '2023-11-01' and l.Data <= '2023-11-30')) as val11,
 (select sum(l.Valor) from lancamento l where (l.IdCategoria = c.Id and l.IdSubcategoria is null and l.Data >= '2023-12-01' and l.Data <= '2023-12-31')) as val12
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

                    obj.val01 = Convert.IsDBNull(result["val01"]) ? null : (decimal?)result["val01"];
                    obj.val02 = Convert.IsDBNull(result["val02"]) ? null : (decimal?)result["val02"];
                    obj.val03 = Convert.IsDBNull(result["val03"]) ? null : (decimal?)result["val03"];
                    obj.val04 = Convert.IsDBNull(result["val04"]) ? null : (decimal?)result["val04"];
                    obj.val05 = Convert.IsDBNull(result["val05"]) ? null : (decimal?)result["val05"];
                    obj.val06 = Convert.IsDBNull(result["val06"]) ? null : (decimal?)result["val06"];
                    obj.val07 = Convert.IsDBNull(result["val07"]) ? null : (decimal?)result["val07"];
                    obj.val08 = Convert.IsDBNull(result["val08"]) ? null : (decimal?)result["val08"];
                    obj.val09 = Convert.IsDBNull(result["val09"]) ? null : (decimal?)result["val09"];
                    obj.val10 = Convert.IsDBNull(result["val10"]) ? null : (decimal?)result["val10"];
                    obj.val11 = Convert.IsDBNull(result["val11"]) ? null : (decimal?)result["val11"];
                    obj.val12 = Convert.IsDBNull(result["val12"]) ? null : (decimal?)result["val12"];

                    list.Add(obj);
                }
            }
            _context.Database.CloseConnection();
            return list;
        }
    }
}