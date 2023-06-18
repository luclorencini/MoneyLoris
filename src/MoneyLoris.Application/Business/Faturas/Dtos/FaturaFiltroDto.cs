using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Business.Faturas.Dtos;
public class FaturaFiltroDto : PaginationFilter
{
    public int IdFatura { get; set; }
}
