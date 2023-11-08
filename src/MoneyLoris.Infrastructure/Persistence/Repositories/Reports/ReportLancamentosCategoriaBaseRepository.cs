using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using MoneyLoris.Application.Reports.LancamentosCategoria.Dto;
using MoneyLoris.Infrastructure.Persistence.Context;

namespace MoneyLoris.Infrastructure.Persistence.Repositories.Reports;
public abstract class ReportLancamentosCategoriaBaseRepository
{
    protected readonly BaseApplicationDbContext _context;

    public ReportLancamentosCategoriaBaseRepository(BaseApplicationDbContext context)
    {
        _context = context;
    }

    protected List<CategoriaQueryResultItemtoDto> ExecutaQueryConsolidado(string query, ReportLancamentoFilterDto filtro)
    {
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

    protected CategoriaQueryResultItemtoDto ConvertToDto(DbDataReader result, ReportLancamentoFilterDto filtro)
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
}
