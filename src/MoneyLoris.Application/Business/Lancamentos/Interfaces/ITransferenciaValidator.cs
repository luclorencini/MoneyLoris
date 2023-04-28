using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Domain.Enums;

namespace MoneyLoris.Application.Business.Lancamentos.Interfaces;
public interface ITransferenciaValidator
{
    void NaoEhAdmin();

    void MeioOrigemNaoPodeSerCartao(MeioPagamento meio);
    void SeTransferenciaEntreContasMeioDestinoNaoPodeSerCartao(TipoTransferencia tipoTransferencia, MeioPagamento meio);
    void SePagamentoFaturaMeioDestinoTemQueSerCartao(TipoTransferencia tipoTransferencia, MeioPagamento meioDestino);

    void OperacaoLancamentoOrigemTemQueSerTransferencia(Lancamento lancamentoOrigem);
    void OperacaoLancamentoDestinoTemQueSerTransferencia(Lancamento lancamentoDestino);

}
