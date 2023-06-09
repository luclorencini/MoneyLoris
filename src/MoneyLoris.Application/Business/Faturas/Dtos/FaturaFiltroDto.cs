using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Business.Faturas.Dtos;
public class FaturaFiltroDto : PaginationFilter
{
    public int IdCartao { get; set; }
    public int Mes { get; set; }
    public int Ano { get; set; }
}
