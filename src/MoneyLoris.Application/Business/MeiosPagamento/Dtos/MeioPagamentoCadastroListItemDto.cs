using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Utils;

namespace MoneyLoris.Application.Business.MeiosPagamento.Dtos;
public class MeioPagamentoCadastroListItemDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = default!;
    public TipoMeioPagamento Tipo { get; set; }
    public string TipoDescricao { get; set; } = default!;
    public string Cor { get; set; } = default!;
    public byte? Ordem { get; set; }
    public bool Ativo { get; set; }
    public decimal Valor { get; set; }

    public MeioPagamentoCadastroListItemDto()
    {
    }

    public MeioPagamentoCadastroListItemDto(MeioPagamento meio)
    {
        Id = meio.Id;
        Nome = meio.Nome;
        Tipo = meio.Tipo;
        TipoDescricao = meio.Tipo.ObterDescricao();
        Cor = meio.Cor;
        Ordem = meio.Ordem;
        Ativo = meio.Ativo;

        Valor = (meio.Tipo == TipoMeioPagamento.CartaoCredito ?
                    (meio.Limite.HasValue ? meio.Limite.Value : 0) :
                    (meio.Saldo.HasValue ? meio.Saldo.Value : 0)
                );
    }
}
