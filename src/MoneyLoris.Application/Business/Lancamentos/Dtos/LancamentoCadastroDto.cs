using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Domain.Enums;

namespace MoneyLoris.Application.Business.Lancamentos.Dtos;
public class LancamentoCadastroDto
{
    public int Id { get; set; }
    public DateTime Data { get; set; }
    public TipoLancamento Tipo { get; set; }
    public int IdMeioPagamento { get; set; }
    public int IdCategoria { get; set; }
    public int? IdSubcategoria { get; set; }
    public string Descricao { get; set; } = default!;
    public decimal Valor { get; set; }

    public LancamentoCadastroDto()
    {
    }

    public LancamentoCadastroDto(Lancamento lancamento)
    {
        Id = lancamento.Id;
        Data = lancamento.Data;
        Tipo = lancamento.Tipo;
        IdMeioPagamento = lancamento.IdMeioPagamento;
        IdCategoria = lancamento.Categoria!.Id;
        IdSubcategoria = lancamento.Subcategoria?.Id;
        Descricao = lancamento.Descricao;
        Valor = lancamento.Valor;
    }
}
