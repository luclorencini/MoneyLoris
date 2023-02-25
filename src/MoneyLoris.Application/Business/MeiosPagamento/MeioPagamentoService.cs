using MoneyLoris.Application.Business.Auth.Interfaces;
using MoneyLoris.Application.Business.MeiosPagamento.Dtos;
using MoneyLoris.Application.Common.Base;
using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Business.MeiosPagamento;
public class MeioPagamentoService : ServiceBase, IMeioPagamentoService
{
    private readonly IMeioPagamentoRepository _meioPagamentoRepo;
    private readonly IAuthenticationManager _authenticationManager;

    public MeioPagamentoService(
        IMeioPagamentoRepository meioPagamentoRepo,
        IAuthenticationManager authenticationManager)
    {
        _meioPagamentoRepo = meioPagamentoRepo;
        _authenticationManager = authenticationManager;
    }

    public async Task<Result<ICollection<MeioPagamentoCadastroListItemDto>>> Listar()
    {
        var userInfo = _authenticationManager.ObterInfoUsuarioLogado();

        var meios = await _meioPagamentoRepo.ListarMeiosPagamentoUsuario(userInfo.Id);

        ICollection<MeioPagamentoCadastroListItemDto> ret =
            meios.Select(m => new MeioPagamentoCadastroListItemDto(m))
            .ToList();

        return new Result<ICollection<MeioPagamentoCadastroListItemDto>>(ret);
    }

    public async Task<Result<MeioPagamentoCadastroDto>> Obter(int id)
    {
        var meio = await obterMeioPagamento(id);

        var dto = new MeioPagamentoCadastroDto(meio);

        return dto;
    }

    private async Task<MeioPagamento> obterMeioPagamento(int id)
    {
        var meio = await _meioPagamentoRepo.GetById(id);

        if (meio == null)
            throw new BusinessException(
                code: ErrorCodes.MeioPagamento_NaoEncontrado,
                message: "Conta ou Cartão não encontrado");

        return meio;
    }

    private bool IsCartao(TipoMeioPagamento tipo)
    {
        return tipo == TipoMeioPagamento.CartaoCredito;
    }

    public async Task<Result<int>> Inserir(MeioPagamentoCriacaoDto dto)
    {
        //TODO - regras de validação

        var userInfo = _authenticationManager.ObterInfoUsuarioLogado();

        var meio = new MeioPagamento
        {
            IdUsuario = userInfo.Id,
            Nome = dto.Nome,
            Tipo = dto.Tipo,
            Cor = dto.Cor,
            Ordem = dto.Ordem,
            Ativo = true,
            Saldo = 0
        };

        if (IsCartao(meio.Tipo))
        {
            meio.Limite = dto.Limite;
            meio.DiaFechamento = dto.DiaFechamento;
            meio.DiaVencimento = dto.DiaVencimento;
        }

        meio = await _meioPagamentoRepo.Insert(meio);

        return (meio.Id,
            IsCartao(meio.Tipo) ?
                "Cartão criado com sucesso." :
                "Conta criada com sucesso.");
    }

    public async Task<Result<int>> Alterar(MeioPagamentoCadastroDto dto)
    {
        //TODO - regras de validação

        var userInfo = _authenticationManager.ObterInfoUsuarioLogado();

        var meio = await obterMeioPagamento(dto.Id);

        if (meio.IdUsuario != userInfo.Id)
            throw new BusinessException(
                code: ErrorCodes.MeioPagamento_NaoPertenceAoUsuario,
                message: "Conta/Cartão não pertence ao usuário.");

        if (IsCartao(meio.Tipo) && !IsCartao(dto.Tipo))
            throw new BusinessException(
                code: ErrorCodes.MeioPagamento_CartaoNaoPodeVirarConta,
                message: "Cartão não pode ter seu tipo alterado para conta.");

        if (!IsCartao(meio.Tipo) && IsCartao(dto.Tipo))
            throw new BusinessException(
                code: ErrorCodes.MeioPagamento_ContaNaoPodeVirarCartao,
                message: "Conta não pode ter seu tipo alterado para cartão.");


        meio.Nome = dto.Nome;
        meio.Tipo = dto.Tipo;
        meio.Cor = dto.Cor;
        meio.Ordem = dto.Ordem;

        if (IsCartao(meio.Tipo))
        {
            meio.Limite = dto.Limite;
            meio.DiaFechamento = dto.DiaFechamento;
            meio.DiaVencimento = dto.DiaVencimento;
        }

        await _meioPagamentoRepo.Update(meio);

        return (meio.Id,
            IsCartao(meio.Tipo) ?
                "Cartão alterado com sucesso." :
                "Conta alterada com sucesso.");
    }

    public async Task<Result<int>> Excluir(int id)
    {
        //TODO - regras de validação

        var userInfo = _authenticationManager.ObterInfoUsuarioLogado();

        var meio = await obterMeioPagamento(id);

        if (meio.IdUsuario != userInfo.Id)
            throw new BusinessException(
                code: ErrorCodes.MeioPagamento_NaoPertenceAoUsuario,
                message: "Conta/Cartão não pertence ao usuário.");

        await _meioPagamentoRepo.Delete(id);

        return (meio.Id,
            IsCartao(meio.Tipo) ?
                "Cartão excluído com sucesso." :
                "Conta excluída com sucesso.");
    }

    public Task<Result> Inativar(int id)
    {
        //TODO
        throw new NotImplementedException();
    }

    public Task<Result> Reativar(int id)
    {
        //TODO
        throw new NotImplementedException();
    }

    public async Task<Result<ICollection<MeioPagamentoListItemDto>>> ObterMeiosPagamento()
    {
        var userInfo = _authenticationManager.ObterInfoUsuarioLogado();

        var meios = await _meioPagamentoRepo.ListarMeiosPagamentoUsuario(userInfo.Id);

        ICollection<MeioPagamentoListItemDto> ret =
            meios.Select(m => new MeioPagamentoListItemDto(m))
            .ToList();

        return new Result<ICollection<MeioPagamentoListItemDto>>(ret);
    }

    public async Task<Result<ICollection<MeioPagamentoListItemDto>>> ObterContas()
    {
        var userInfo = _authenticationManager.ObterInfoUsuarioLogado();

        var meios = await _meioPagamentoRepo.ListarContasUsuario(userInfo.Id);

        ICollection<MeioPagamentoListItemDto> ret =
            meios.Select(m => new MeioPagamentoListItemDto(m))
            .ToList();

        return new Result<ICollection<MeioPagamentoListItemDto>>(ret);
    }

    public async Task<Result<ICollection<MeioPagamentoListItemDto>>> ObterCartoes()
    {
        var userInfo = _authenticationManager.ObterInfoUsuarioLogado();

        var meios = await _meioPagamentoRepo.ListarCartoesUsuario(userInfo.Id);

        ICollection<MeioPagamentoListItemDto> ret =
            meios.Select(m => new MeioPagamentoListItemDto(m))
            .ToList();

        return new Result<ICollection<MeioPagamentoListItemDto>>(ret);
    }    
}
