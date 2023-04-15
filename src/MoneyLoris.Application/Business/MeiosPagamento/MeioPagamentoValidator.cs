using MoneyLoris.Application.Business.Auth.Interfaces;
using MoneyLoris.Application.Business.Lancamentos;
using MoneyLoris.Application.Business.MeiosPagamento.Dtos;
using MoneyLoris.Application.Business.MeiosPagamento.Interfaces;
using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Business.MeiosPagamento;
public class MeioPagamentoValidator : IMeioPagamentoValidator
{
    private readonly UserAuthInfo userInfo;
    private readonly ILancamentoRepository _lancamentoRepo;

    public MeioPagamentoValidator(
        IAuthenticationManager authenticationManager,
        ILancamentoRepository lancamentoRepo
    )
    {
        userInfo = authenticationManager.ObterInfoUsuarioLogado();
        _lancamentoRepo = lancamentoRepo;
    }


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

    public void PertenceAoUsuario(MeioPagamento meio)
    {
        if (meio.IdUsuario != userInfo.Id)
            throw new BusinessException(
                code: ErrorCodes.MeioPagamento_NaoPertenceAoUsuario,
                message: "Conta/Cartão não pertence ao usuário");
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
