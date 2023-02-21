using MoneyLoris.Application.Domain.Enums;

namespace MoneyLoris.Application.Business.MeiosPagamento.Dtos;
public class MeioPagamentoCriacaoDto
{
    public string Nome { get; set; } = default!;
    public TipoMeioPagamento Tipo { get; set; }
    public string Cor { get; set; } = default!;
    public int? Ordem { get; set; }

    //public decimal SaldoInicial { get; set; }
    //public DateTime DataLancamentoInicial { get; set; }

    public decimal Limite { get; set; }
    public byte DiaFechamento { get; set; }
    public byte DiaVencimento { get; set; }
}
