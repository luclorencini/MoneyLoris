using MoneyLoris.Application.Domain.Entities;
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

    public int? FaturaMes { get; set; }
    public int? FaturaAno { get; set; }

    public TransferenciaUpdateDto()
    {
    }

    public TransferenciaUpdateDto(Lancamento lancamentoOrigem, Lancamento lancamentoDestino, Fatura? fatura)
    {
        IdLancamentoOrigem = lancamentoOrigem.Id;
        IdLancamentoDestino = lancamentoDestino.Id;

        Data = lancamentoOrigem.Data;
        Tipo = lancamentoOrigem.TipoTransferencia!.Value;

        IdMeioPagamentoOrigem = lancamentoOrigem.IdMeioPagamento;
        IdMeioPagamentoDestino = lancamentoDestino.IdMeioPagamento;

        Valor = Math.Abs(lancamentoOrigem.Valor); //garante que o valor vai sempre positivo pra interface

        if (fatura is not null)
        {
            FaturaMes = fatura.Mes;
            FaturaAno = fatura.Ano;
        }
    }
}
