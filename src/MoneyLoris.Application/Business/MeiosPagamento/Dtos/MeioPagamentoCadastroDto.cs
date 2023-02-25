using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Domain.Enums;

namespace MoneyLoris.Application.Business.MeiosPagamento.Dtos;
public class MeioPagamentoCadastroDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = default!;
    public TipoMeioPagamento Tipo { get; set; }
    public string Cor { get; set; } = default!;
    public byte? Ordem { get; set; }
    public bool Ativo { get; set; }

    public decimal? Limite { get; set; }
    public byte? DiaFechamento { get; set; }
    public byte? DiaVencimento { get; set; }

    public MeioPagamentoCadastroDto()
    {
    }

    public MeioPagamentoCadastroDto(MeioPagamento meio)
    {
        Id = meio.Id;
        Nome = meio.Nome;
        Tipo = meio.Tipo;
        Cor = meio.Cor;
        Ordem = meio.Ordem;
        Ativo = meio.Ativo;

        Limite = meio.Limite;
        DiaFechamento = meio.DiaFechamento;
        DiaVencimento = meio.DiaVencimento;
    }
}
