namespace MoneyLoris.Application.Business.Lancamentos.Dtos;
public class TransferenciaInsertDto
{
    public DateTime Data { get; set; }
    public int IdMeioPagamentoOrigem { get; set; }
    public int IdMeioPagamentoDestino { get; set; }
    public decimal Valor { get; set; }

    //campos para cartão de crédito
    public int? FaturaMes { get; set; }
    public int? FaturaAno { get; set; }
}
