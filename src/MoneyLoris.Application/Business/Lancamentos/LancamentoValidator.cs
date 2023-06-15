using MoneyLoris.Application.Business.Auth.Interfaces;
using MoneyLoris.Application.Business.Lancamentos.Dtos;
using MoneyLoris.Application.Business.Lancamentos.Interfaces;
using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Business.Lancamentos;
public class LancamentoValidator : ILancamentoValidator
{
    private readonly IAuthenticationManager _authenticationManager;

    public LancamentoValidator(IAuthenticationManager authenticationManager)
    {
        _authenticationManager = authenticationManager;
    }

    private UserAuthInfo userInfo => _authenticationManager.ObterInfoUsuarioLogado();

    public void NaoEhAdmin()
    {
        if (userInfo.IsAdmin)
            throw new BusinessException(
                code: ErrorCodes.Lancamento_AdminNaoPode,
                message: "Administradores não possuem lançamentos");
    }

    public void Existe(Lancamento lancamento)
    {
        if (lancamento == null)
            throw new BusinessException(
                code: ErrorCodes.Lancamento_NaoEncontrado,
                message: "Lançamento não encontrado");
    }

    public void OrigemExiste(Lancamento lancamentoOrigem)
    {
        if (lancamentoOrigem == null)
            throw new BusinessException(
                code: ErrorCodes.Lancamento_OrigemNaoEncontrado,
                message: "Lançamento origem não encontrado");
    }

    public void DestinoExiste(Lancamento lancamentoDestino)
    {
        if (lancamentoDestino == null)
            throw new BusinessException(
                code: ErrorCodes.Lancamento_DestinoNaoEncontrado,
                message: "Lançamento destino não encontrado");
    }


    public void PertenceAoUsuario(Lancamento lancamento)
    {
        if (lancamento.IdUsuario != userInfo.Id)
            throw new BusinessException(
                code: ErrorCodes.Lancamento_NaoPertenceAoUsuario,
                message: "Lançamento não pertence ao usuário");
    }

    public void OrigemPertenceAoUsuario(Lancamento lancamentoOrigem)
    {
        if (lancamentoOrigem.IdUsuario != userInfo.Id)
            throw new BusinessException(
                code: ErrorCodes.Lancamento_OrigemNaoPertenceAoUsuario,
                message: "Lançamento origem não pertence ao usuário");
    }

    public void DestinoPertenceAoUsuario(Lancamento lancamentoDestino)
    {
        if (lancamentoDestino.IdUsuario != userInfo.Id)
            throw new BusinessException(
                code: ErrorCodes.Lancamento_DestinoNaoPertenceAoUsuario,
                message: "Lançamento destino não pertence ao usuário");
    }


    public void EstaConsistente(Lancamento lancamento)
    {
        if (lancamento is null)
            throw new BusinessException(
                code: ErrorCodes.Lancamento_CamposObrigatorios,
                message: "Lançamento não informado");

        if (lancamento.IdUsuario <= 0)
            throw new BusinessException(
                code: ErrorCodes.Lancamento_CamposObrigatorios,
                message: "Usuário nao informado");

        if (lancamento.IdMeioPagamento <= 0)
            throw new BusinessException(
                code: ErrorCodes.Lancamento_CamposObrigatorios,
                message: "Meio de Pagamento não informado");

        if (String.IsNullOrWhiteSpace(lancamento.Descricao))
            throw new BusinessException(
                code: ErrorCodes.Lancamento_CamposObrigatorios,
                message: "Descrição não informada");

        //valor
        if (lancamento.Tipo == TipoLancamento.Receita &&
            lancamento.Valor <= 0)
            throw new BusinessException(
                code: ErrorCodes.Lancamento_CamposObrigatorios,
                message: "Lançamento de receita deve ter valor positivo");

        if (lancamento.Tipo == TipoLancamento.Despesa &&
            lancamento.Valor >= 0)
            throw new BusinessException(
                code: ErrorCodes.Lancamento_CamposObrigatorios,
                message: "Lançamento de despesa deve ter valor negativo");

        //lançamento simples
        if (lancamento.Operacao == OperacaoLancamento.LancamentoSimples &&
            lancamento.IdLancamentoTransferencia is not null)
            throw new BusinessException(
                code: ErrorCodes.Lancamento_CamposObrigatorios,
                message: "Apenas lançamento de transferência possuem lançamento relacionado");

        if (lancamento.Operacao == OperacaoLancamento.LancamentoSimples &&
            lancamento.TipoTransferencia is not null)
            throw new BusinessException(
                code: ErrorCodes.Lancamento_CamposObrigatorios,
                message: "Tipo de transferência não se aplica a lançamento simples");

        //transferência
        if (lancamento.Operacao == OperacaoLancamento.Transferencia &&
            lancamento.IdLancamentoTransferencia is null)
            throw new BusinessException(
                code: ErrorCodes.Lancamento_CamposObrigatorios,
                message: "Lançamento que compõe transferência precisa ter lançamento relacionado");

        if (lancamento.Operacao == OperacaoLancamento.Transferencia &&
            lancamento.TipoTransferencia is null)
            throw new BusinessException(
                code: ErrorCodes.Lancamento_CamposObrigatorios,
                message: "Lançamento que compõe transferência precisa definir o tipo da transferência");

        //parcelas
        if ((lancamento.ParcelaAtual is null && lancamento.ParcelaTotal is not null) ||
            (lancamento.ParcelaAtual is not null && lancamento.ParcelaTotal is null))
            throw new BusinessException(
                code: ErrorCodes.Lancamento_CamposObrigatorios,
                message: "Parcela atual e o Total de parcelas precisam estar preenchidas");

        if (lancamento.ParcelaAtual is not null && lancamento.ParcelaTotal is not null &&
            lancamento.ParcelaAtual > lancamento.ParcelaTotal)
            throw new BusinessException(
                code: ErrorCodes.Lancamento_CamposObrigatorios,
                message: "Parcela atual não pode ser maior que o total de parcelas");

        //TODO - fatura
    }


    public void LancamentoCartaoCreditoTemQueTerParcela(MeioPagamento meio, short? parcelas)
    {
        if (meio.Tipo == TipoMeioPagamento.CartaoCredito && parcelas is null)
            throw new BusinessException(
                code: ErrorCodes.Lancamento_CartaoCreditoSemParcela,
                message: "Não é possível lançar uma despesa de Cartão de Crédito sem informar número de parcelas");
    }

    public void TipoLancamentoIgualTipoCategoria(TipoLancamento tipo, Categoria categoria)
    {
        if (categoria.Tipo != tipo)
            throw new BusinessException(
                code: ErrorCodes.Lancamento_TipoDiferenteDaCategoria,
                message: "Tipo do lançamento é diferente do tipo da categoria.");
    }

    public void NaoPodeTrocarMeioPagamento(Lancamento lancamento, int idMeioPagamentoInformado)
    {
        if (lancamento.IdMeioPagamento != idMeioPagamentoInformado)
            throw new BusinessException(
                code: ErrorCodes.MeioPagamento_TipoDiferenteAlteracao,
                message: "Não é possível trocar o meio de pagamento na alteração");
    }

    public void LancamentoCartaoCreditoTemQueTerFatura(MeioPagamento meio, int? idFatura)
    {
        throw new NotImplementedException();
    }
}
