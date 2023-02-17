using MoneyLoris.Application.Domain.Enums;

namespace MoneyLoris.Application.Business.MeiosPagamento.Dtos;
public class MeioPagamentoCadastroListItemDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = default!;
    public TipoMeioPagamento Tipo { get; set; }
    public string TipoDescricao { get; set; } = default!;
    public string Cor { get; set; } = default!;
    public int? Ordem { get; set; }
    public bool Ativo { get; set; }
    public decimal Valor { get; set; }
}
