using MoneyLoris.Application.Business.Lancamentos.Dtos;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Business.Faturas.Dtos;
public class FaturaPesquisaDto
{
    public FaturaInfoDto Info { get; set; }
    public Pagination<ICollection<LancamentoListItemDto>> Listagem { get; set; }
}
