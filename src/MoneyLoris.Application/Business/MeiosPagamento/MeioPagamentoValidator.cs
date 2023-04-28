using MoneyLoris.Application.Business.Auth.Interfaces;
using MoneyLoris.Application.Business.Lancamentos.Interfaces;
using MoneyLoris.Application.Business.MeiosPagamento.Dtos;
using MoneyLoris.Application.Business.MeiosPagamento.Interfaces;
using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Business.MeiosPagamento;
public class MeioPagamentoValidator : IMeioPagamentoValidator
{
    private readonly IAuthenticationManager _authenticationManager;
    private readonly ILancamentoRepository _lancamentoRepo;

    public MeioPagamentoValidator(
        IAuthenticationManager authenticationManager,
        ILancamentoRepository lancamentoRepo
    )
    {
        _authenticationManager = authenticationManager;
        _lancamentoRepo = lancamentoRepo;
    }

    private UserAuthInfo userInfo => _authenticationManager.ObterInfoUsuarioLogado();


    public void NaoEhAdmin()
    {
        if (userInfo.IsAdmin)
            throw new BusinessException(
                code: ErrorCodes.MeioPagamento_AdminNaoPode,
                message: "Administradores não possuem meios de pagamento");
    }


    public void Existe(MeioPagamento meio)
    {
        if (meio == null)
            throw new BusinessException(
                code: ErrorCodes.MeioPagamento_NaoEncontrado,
                message: "Conta ou Cartão não encontrado");
    }

    public void OrigemExiste(MeioPagamento meioOrigem)
    {
        if (meioOrigem == null)
            throw new BusinessException(
                code: ErrorCodes.MeioPagamento_OrigemNaoEncontrado,
                message: "Conta ou Cartão origem não encontrado");
    }

    public void DestinoExiste(MeioPagamento meioDestino)
    {
        if (meioDestino == null)
            throw new BusinessException(
                code: ErrorCodes.MeioPagamento_DestinoNaoEncontrado,
                message: "Conta ou Cartão destino não encontrado");
    }


    public void PertenceAoUsuario(MeioPagamento meio)
    {
        if (meio.IdUsuario != userInfo.Id)
            throw new BusinessException(
                code: ErrorCodes.MeioPagamento_NaoPertenceAoUsuario,
                message: "Conta/Cartão não pertence ao usuário");
    }

    public void OrigemPertenceAoUsuario(MeioPagamento meioOrigem)
    {
        if (meioOrigem.IdUsuario != userInfo.Id)
            throw new BusinessException(
                code: ErrorCodes.MeioPagamento_OrigemNaoPertenceAoUsuario,
                message: "Conta/Cartão origem não pertence ao usuário.");
    }

    public void DestinoPertenceAoUsuario(MeioPagamento meioDestino)
    {
        if (meioDestino.IdUsuario != userInfo.Id)
            throw new BusinessException(
                code: ErrorCodes.MeioPagamento_DestinoNaoPertenceAoUsuario,
                message: "Conta/Cartão destino não pertence ao usuário.");
    }


    public void Ativo(MeioPagamento meio)
    {
        if (!meio.Ativo)
            throw new BusinessException(
                code: ErrorCodes.MeioPagamento_Inativo,
                message: "Conta/Cartão está inativo");
    }

    public void EstaConsistente(MeioPagamento meio)
    {
        if (meio is null)
            throw new BusinessException(
                code: ErrorCodes.MeioPagamento_CamposObrigatorios,
                message: "Meio de Pagamento nao informado");

        if (meio.IdUsuario <= 0)
            throw new BusinessException(
                code: ErrorCodes.MeioPagamento_CamposObrigatorios,
                message: "Usuário nao informado");

        if (String.IsNullOrWhiteSpace(meio.Nome))
            throw new BusinessException(
                code: ErrorCodes.MeioPagamento_CamposObrigatorios,
                message: "Nome nao informado");

        if (String.IsNullOrWhiteSpace(meio.Cor))
            throw new BusinessException(
                code: ErrorCodes.MeioPagamento_CamposObrigatorios,
                message: "Cor nao informada");

        if (IsCartao(meio.Tipo))
        {
            if (meio.Limite is null)
                throw new BusinessException(
                    code: ErrorCodes.MeioPagamento_CamposObrigatorios,
                    message: "Limite é obrigatório para cartões de crédito");

            if (meio.DiaFechamento is null)
                throw new BusinessException(
                    code: ErrorCodes.MeioPagamento_CamposObrigatorios,
                    message: "Dia de Fechamento é obrigatório para cartões de crédito");

            if (meio.DiaVencimento is null)
                throw new BusinessException(
                    code: ErrorCodes.MeioPagamento_CamposObrigatorios,
                    message: "Dia de Vencimento é obrigatório para cartões de crédito");

            if (meio.Saldo != 0)
                throw new BusinessException(
                    code: ErrorCodes.MeioPagamento_CamposObrigatorios,
                    message: "Saldo de Cartão de Crédito deve ser sempre zero");
        }
    }

    private bool IsCartao(TipoMeioPagamento tipo)
    {
        return tipo == TipoMeioPagamento.CartaoCredito;
    }

    public void NaoPodeMudarDeContaPraCartaoOuViceVersa(MeioPagamento meio, MeioPagamentoCadastroDto dto)
    {
        if (IsCartao(meio.Tipo) && !IsCartao(dto.Tipo))
            throw new BusinessException(
                code: ErrorCodes.MeioPagamento_CartaoNaoPodeVirarConta,
                message: "Cartão não pode ter seu tipo alterado para conta.");

        if (!IsCartao(meio.Tipo) && IsCartao(dto.Tipo))
            throw new BusinessException(
                code: ErrorCodes.MeioPagamento_ContaNaoPodeVirarCartao,
                message: "Conta não pode ter seu tipo alterado para cartão.");
    }

    public async Task NaoPossuiLancamentos(MeioPagamento meio)
    {
        var qtdeLancamentos = await _lancamentoRepo.QuantidadePorMeioPagamento(meio.Id);

        if (qtdeLancamentos > 0)
            throw new BusinessException(
                code: ErrorCodes.MeioPagamento_PossuiLancamentos,
                message: "Conta/Cartão possui lançamentos");
    }

}
