using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Utils;

namespace MoneyLoris.Application.Business.MeiosPagamento.Dtos;
public class MeioPagamentoListItemDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = default!;
    public TipoMeioPagamento Tipo { get; set; }
    public string TipoDescricao { get; set; } = default!;
    public string Cor { get; set; } = default!;

    public MeioPagamentoListItemDto()
    {
    }

    public MeioPagamentoListItemDto(MeioPagamento meio)
    {
        Id = meio.Id;
        Nome = meio.Nome;
        Tipo = meio.Tipo;
        TipoDescricao = meio.Tipo.ObterDescricao();
        Cor = meio.Cor;
    }
}
