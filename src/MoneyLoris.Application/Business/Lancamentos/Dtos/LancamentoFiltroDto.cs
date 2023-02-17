using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Business.Lancamentos.Dtos;
public class LancamentoFiltroDto : PaginationFilter
{
    public string Descricao { get; set; } = default!;

    public DateTime? DataInicio { get; set; }

    public DateTime? DataFim { get; set; }

    public int? IdMeioPagamento { get; set; }

    public int? IdCategoria { get; set; }

    public int? IdSubcategoria { get; set; }
}
