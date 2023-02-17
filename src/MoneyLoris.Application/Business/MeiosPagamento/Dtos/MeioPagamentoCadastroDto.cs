using MoneyLoris.Application.Domain.Enums;

namespace MoneyLoris.Application.Business.MeiosPagamento.Dtos;
public class MeioPagamentoCadastroDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = default!;
    public byte Tipo { get; set; }
    public string Cor { get; set; } = default!;
    public int? Ordem { get; set; }
    public bool Ativo { get; set; }

    public decimal Limite { get; set; }
    public byte DiaFechamento { get; set; }
    public byte DiaVencimento { get; set; }
}
