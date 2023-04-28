using MoneyLoris.Application.Business.Auth.Interfaces;
using MoneyLoris.Application.Business.Lancamentos.Interfaces;
using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Business.Lancamentos;
public class TransferenciaValidator : ITransferenciaValidator
{
    private readonly IAuthenticationManager _authenticationManager;

    public TransferenciaValidator(IAuthenticationManager authenticationManager)
    {
        _authenticationManager = authenticationManager;
    }

    private UserAuthInfo userInfo => _authenticationManager.ObterInfoUsuarioLogado();

    public void NaoEhAdmin()
    {
        if (userInfo.IsAdmin)
            throw new BusinessException(
                code: ErrorCodes.Transferencia_AdminNaoPode,
                message: "Administradores não realizam transferências");
    }

    public void MeioOrigemNaoPodeSerCartao(MeioPagamento meio)
    {
        if (meio.Tipo == TipoMeioPagamento.CartaoCredito)
            throw new BusinessException(
                code: ErrorCodes.Transferencia_MeioOrigemNaoPodeSerCartao,
                message: "Origem da transferência não pode ser Cartão de Crédito.");
    }

    public void SeTransferenciaEntreContasMeioDestinoNaoPodeSerCartao(TipoTransferencia tipoTransferencia, MeioPagamento meioDestino)
    {
        if (tipoTransferencia == TipoTransferencia.TransferenciaEntreContas &&
            meioDestino.Tipo == TipoMeioPagamento.CartaoCredito)
            throw new BusinessException(
                code: ErrorCodes.Transferencia_EntreContasDestinoNaoPodeSerCartao,
                message: "Destino da transferência entre contas não pode ser Cartão de Crédito.");
    }

    public void SePagamentoFaturaMeioDestinoTemQueSerCartao(TipoTransferencia tipoTransferencia, MeioPagamento meioDestino)
    {
        if (tipoTransferencia == TipoTransferencia.PagamentoFatura &&
            meioDestino.Tipo != TipoMeioPagamento.CartaoCredito)
            throw new BusinessException(
                code: ErrorCodes.Transferencia_PagamentoFaturaDestinoTemQueSerCartao,
                message: "Destino do pagamento de fatura não pode ser uma Conta.");
    }

    public void OperacaoLancamentoOrigemTemQueSerTransferencia(Lancamento lancamentoOrigem)
    {
        if (lancamentoOrigem.Operacao != OperacaoLancamento.Transferencia)
            throw new BusinessException(
                code: ErrorCodes.Transferencia_OperacaoLancamentoOrigemNaoEhTransferencia,
                message: "Lançamento origem não é uma transferência / pagamento de fatura");
    }

    public void OperacaoLancamentoDestinoTemQueSerTransferencia(Lancamento lancamentoDestino)
    {
        if (lancamentoDestino.Operacao != OperacaoLancamento.Transferencia)
            throw new BusinessException(
                code: ErrorCodes.Transferencia_OperacaoLancamentoDestinoNaoEhTransferencia,
                message: "Lançamento destino não é uma transferência / pagamento de fatura");
    }
}
