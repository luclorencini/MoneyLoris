using MoneyLoris.Application.Domain.Enums;

namespace MoneyLoris.Application.Business.Lancamentos.Dtos;
public class LancamentoListItemDto
{
    public int Id { get; set; }
    public DateTime Data { get; set; }
    public TipoLancamento Tipo { get; set; }
    public OperacaoLancamento Operacao { get; set; }

    public string MeioPagamentoNome { get; set; } = default!;
    public TipoMeioPagamento MeioPagamentoTipo { get; set; } = default!;
    public string MeioPagamentoCor { get; set; } = default!;
    
    public string Categoria { get; set; } = default!;
    public string Subcategoria { get; set; } = default!;

    public string Descricao { get; set; } = default!;
    public decimal Valor { get; set; }
}
