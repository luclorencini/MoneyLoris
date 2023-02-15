using MoneyLoris.Application.Domain.Enums;

namespace MoneyLoris.Application.Business.Contas.Dtos;
public class ContaCadastroDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = default!;
    public TipoConta Tipo { get; set; }
    public string Cor { get; set; } = default!;
    public int? Ordem { get; set; }
    public bool Ativo { get; set; }

    public double Limite { get; set; }
    public byte DiaFechamento { get; set; }
    public byte DiaVencimento { get; set; }
}
