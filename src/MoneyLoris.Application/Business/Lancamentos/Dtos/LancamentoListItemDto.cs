using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Domain.Enums;

namespace MoneyLoris.Application.Business.Lancamentos.Dtos;
public class LancamentoListItemDto
{
    public int Id { get; set; }
    public DateTime Data { get; set; }
    public TipoLancamento Tipo { get; set; }
    public OperacaoLancamento Operacao { get; set; }

    public string MeioPagamentoNome { get; set; } = default!;
    public TipoMeioPagamento MeioPagamentoTipo { get; set; } = default!;
    public string MeioPagamentoCor { get; set; } = default!;

    public string Categoria { get; set; } = default!;
    public string Subcategoria { get; set; } = default!;

    public string Descricao { get; set; } = default!;
    public decimal Valor { get; set; }

    public int? IdLancamentoOrigemTransferencia { get; set; }

    public short? ParcelaAtual { get; set; }
    public short? ParcelaTotal { get; set; }


    public LancamentoListItemDto()
    {
    }

    public LancamentoListItemDto(Lancamento lancamento)
    {
        Id = lancamento.Id;
        Data = lancamento.Data;
        Tipo = lancamento.Tipo;
        Operacao = lancamento.Operacao;

        MeioPagamentoNome = lancamento.MeioPagamento.Nome;
        MeioPagamentoTipo = lancamento.MeioPagamento.Tipo;
        MeioPagamentoCor = lancamento.MeioPagamento.Cor;

        Descricao = lancamento.Descricao;
        Valor = lancamento.Valor;

        ParcelaAtual = lancamento.ParcelaAtual;
        ParcelaTotal = lancamento.ParcelaTotal;

        if (Operacao == OperacaoLancamento.LancamentoSimples)
        {
            Categoria = lancamento!.Categoria!.Nome;
            Subcategoria = lancamento!.Subcategoria?.Nome;
        }
        else
        {
            Categoria = (lancamento.TipoTransferencia == TipoTransferencia.TransferenciaEntreContas ? "Transferência" : "Pagamento de Fatura");

            //sempre traz o id do lançamento origem da transferência (a despesa)
            IdLancamentoOrigemTransferencia =
                (lancamento.Tipo == TipoLancamento.Despesa ?
                    lancamento.Id :
                    lancamento.IdLancamentoTransferencia);
        }



    }
}
