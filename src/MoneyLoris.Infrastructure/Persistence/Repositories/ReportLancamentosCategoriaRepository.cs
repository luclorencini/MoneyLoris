using System.Data;
using Microsoft.EntityFrameworkCore;
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


    public List<CategoriaGroupItemtoDto> RelatorioLancamentosPorCategoria(int mes, int ano, int quantidade)
    {
        var query = @"
select c.Id as catId, c.Nome as catNome, c.Ordem as catOrdem, s.Id as subId, s.Nome as subNome, s.Ordem as subOrdem,
 (select sum(l.Valor) from lancamento l where (l.IdCategoria = c.Id and l.IdSubcategoria = s.Id and l.Data >= '2023-01-01' and l.Data <= '2023-01-31')) as jan,
 (select sum(l.Valor) from lancamento l where (l.IdCategoria = c.Id and l.IdSubcategoria = s.Id and l.Data >= '2023-02-01' and l.Data <= '2023-02-28')) as fev,
 (select sum(l.Valor) from lancamento l where (l.IdCategoria = c.Id and l.IdSubcategoria = s.Id and l.Data >= '2023-03-01' and l.Data <= '2023-03-31')) as mar
from categoria c
inner join subcategoria s on s.IdCategoria = c.Id
where c.Tipo = 2 and c.IdUsuario = 3
UNION
select c.Id as catId, c.Nome as catNome, c.Ordem as catOrdem, NULL as subId, NULL as subNome, NULL as subOrdem,
 (select sum(l.Valor) from lancamento l where (l.IdCategoria = c.Id and l.IdSubcategoria is null and l.Data >= '2023-01-01' and l.Data <= '2023-01-31')) as jan,
 (select sum(l.Valor) from lancamento l where (l.IdCategoria = c.Id and l.IdSubcategoria is null and l.Data >= '2023-02-01' and l.Data <= '2023-02-28')) as fev,
 (select sum(l.Valor) from lancamento l where (l.IdCategoria = c.Id and l.IdSubcategoria is null and l.Data >= '2023-03-01' and l.Data <= '2023-03-31')) as mar
from categoria c
where c.Tipo = 2 and c.IdUsuario = 3
order by catOrdem is null, catOrdem, catNome, subNome is not null, subOrdem is null, subOrdem
";


            var conn = _context.Database.GetDbConnection();

        using (var command = _context.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = query;
            command.CommandType = CommandType.Text;
            _context.Database.OpenConnection();

            var list = new List<CategoriaGroupItemtoDto>();
            using (var result = command.ExecuteReader())
            {
                while (result.Read())
                {
                    var obj = new CategoriaGroupItemtoDto();

                    obj.catId = Convert.IsDBNull(result["catId"]) ? null : (int?)result["catId"];
                    obj.catNome = Convert.IsDBNull(result["catNome"]) ? null : (string?)result["catNome"];
                    obj.catOrdem = Convert.IsDBNull(result["catOrdem"]) ? null : (sbyte?)result["catOrdem"];

                    obj.subId = Convert.IsDBNull(result["subId"]) ? null : (int?)result["subId"];
                    obj.subNome = Convert.IsDBNull(result["subNome"]) ? null : (string?)result["subNome"];
                    obj.subOrdem = Convert.IsDBNull(result["subOrdem"]) ? null : (sbyte?)result["subOrdem"];

                    obj.jan = Convert.IsDBNull(result["jan"]) ? null : (decimal?)result["jan"];
                    obj.fev = Convert.IsDBNull(result["fev"]) ? null : (decimal?)result["fev"];
                    obj.mar = Convert.IsDBNull(result["mar"]) ? null : (decimal?)result["mar"];

                    list.Add(obj);
                }
            }
            _context.Database.CloseConnection();
            return list;
        }
    }
}