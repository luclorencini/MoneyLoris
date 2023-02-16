namespace MoneyLoris.Application.Business.Contas.Dtos;
public class ContaCadastroListItemDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = default!;
    public byte Tipo { get; set; }
    public string TipoDescricao { get; set; } = default!;
    public string Cor { get; set; } = default!;
    public int? Ordem { get; set; }
    public bool Ativo { get; set; }

    public double Valor { get; set; }
}
