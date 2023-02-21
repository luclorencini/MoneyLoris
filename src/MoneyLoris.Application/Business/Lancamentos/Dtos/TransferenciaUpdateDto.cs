using MoneyLoris.Application.Domain.Enums;

namespace MoneyLoris.Application.Business.Lancamentos.Dtos;
public class TransferenciaUpdateDto
{
    public int IdLancamentoOrigem { get; set; }
    public int IdLancamentoDestino { get; set; }

    public DateTime Data { get; set; }
    public TipoTransferencia Tipo { get; set; }
    public int IdMeioPagamentoOrigem { get; set; }
    public int IdMeioPagamentoDestino { get; set; }
    public decimal Valor { get; set; }
}
