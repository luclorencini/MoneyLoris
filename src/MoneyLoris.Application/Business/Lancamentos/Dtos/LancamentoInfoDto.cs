using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Domain.Enums;

namespace MoneyLoris.Application.Business.Lancamentos.Dtos;
public class LancamentoInfoDto
{
    public int Id { get; set; }
    public DateTime Data { get; set; }
    public TipoLancamento Tipo { get; set; }

    public int IdMeioPagamento { get; set; }

    public int IdCategoria { get; set; }
    public int? IdSubcategoria { get; set; }

    public string Descricao { get; set; } = default!;
    public decimal Valor { get; set; }

    public short? ParcelaAtual { get; set; }
    public short? ParcelaTotal { get; set; }
    public int? FaturaMes { get; set; }
    public int? FaturaAno { get; set; }

    public LancamentoInfoDto()
    {
    }

    public LancamentoInfoDto(Lancamento lancamento)
    {
        Id = lancamento.Id;
        Data = lancamento.Data;
        Tipo = lancamento.Tipo;

        IdMeioPagamento = lancamento.MeioPagamento.Id;

        IdCategoria = lancamento.IdCategoria!.Value;
        IdSubcategoria = lancamento.IdSubcategoria;
        Descricao = lancamento.Descricao;
        Valor = lancamento.Valor;
        ParcelaAtual = lancamento.ParcelaAtual;
        ParcelaTotal = lancamento.ParcelaTotal;

        FaturaMes = lancamento.Fatura?.Mes;
        FaturaAno = lancamento.Fatura?.Ano;
    }
}
