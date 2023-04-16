using MoneyLoris.Application.Business.Auth.Interfaces;
using MoneyLoris.Application.Business.Lancamentos.Dtos;
using MoneyLoris.Application.Business.Lancamentos.Interfaces;
using MoneyLoris.Application.Business.MeiosPagamento.Interfaces;
using MoneyLoris.Application.Common.Base;
using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Shared;
using MoneyLoris.Application.Utils;

namespace MoneyLoris.Application.Business.Lancamentos;
public class TransferenciaService : ServiceBase, ITransferenciaService
{
    private readonly ILancamentoRepository _lancamentoRepo;
    private readonly IMeioPagamentoRepository _meioPagamentoRepo;
    private readonly IAuthenticationManager _authenticationManager;

    public TransferenciaService(
        ILancamentoRepository lancamentoRepo,
        IMeioPagamentoRepository meioPagamentoRepo,
        IAuthenticationManager authenticationManager)
    {
        _lancamentoRepo = lancamentoRepo;
        _meioPagamentoRepo = meioPagamentoRepo;
        _authenticationManager = authenticationManager;
    }

    public async Task<Result<int>> InserirTransferenciaEntreContas(TransferenciaInsertDto dto)
    {
        return await InserirTransferenciaTransactional(dto, TipoTransferencia.TransferenciaEntreContas);
    }

    public async Task<Result<int>> InserirPagamentoFatura(TransferenciaInsertDto dto)
    {
        return await InserirTransferenciaTransactional(dto, TipoTransferencia.PagamentoFatura);
    }

    private async Task<Result<int>> InserirTransferenciaTransactional(TransferenciaInsertDto dto, TipoTransferencia tipoTransferencia)
    {
        var userInfo = _authenticationManager.ObterInfoUsuarioLogado();

        // validações meio origem

        var meioOrigem = await _meioPagamentoRepo.GetById(dto.IdMeioPagamentoOrigem);

        if (meioOrigem == null)
            throw new BusinessException(
                code: ErrorCodes.MeioPagamento_NaoEncontrado,
                message: "Conta ou Cartão origem não encontrado");

        if (meioOrigem.IdUsuario != userInfo.Id)
            throw new BusinessException(
                code: ErrorCodes.MeioPagamento_NaoPertenceAoUsuario,
                message: "Conta/Cartão origem não pertence ao usuário.");

        if (meioOrigem.Tipo == TipoMeioPagamento.CartaoCredito)
            throw new BusinessException(
                code: ErrorCodes.Lancamento_TransferenciaOrigemNaoPodeSerCartao,
                message: "Origem da transferência não pode ser Cartão de Crédito.");

        // validações meio destino

        var meioDestino = await _meioPagamentoRepo.GetById(dto.IdMeioPagamentoDestino);

        if (meioDestino == null)
            throw new BusinessException(
                code: ErrorCodes.MeioPagamento_NaoEncontrado,
                message: "Conta ou Cartão destino não encontrado");

        if (meioDestino.IdUsuario != userInfo.Id)
            throw new BusinessException(
                code: ErrorCodes.MeioPagamento_NaoPertenceAoUsuario,
                message: "Conta/Cartão destino não pertence ao usuário.");

        if (tipoTransferencia == TipoTransferencia.TransferenciaEntreContas && meioDestino.Tipo == TipoMeioPagamento.CartaoCredito)
            throw new BusinessException(
                code: ErrorCodes.Lancamento_TransferenciaEntreContasDestinoNaoPodeSerCartao,
                message: "Destino da transferência entre contas não pode ser Cartão de Crédito.");

        if (tipoTransferencia == TipoTransferencia.PagamentoFatura && meioDestino.Tipo != TipoMeioPagamento.CartaoCredito)
            throw new BusinessException(
                code: ErrorCodes.Lancamento_PagamentoFaturaDestinoNaoPodeSerConta,
                message: "Destino do pagamento de fatura não pode ser uma Conta.");

        // setup

        string descricaoOrigem = null!;
        string descricaoDestino = null!;

        if (tipoTransferencia == TipoTransferencia.TransferenciaEntreContas)
        {
            descricaoOrigem = $"Transferência para {meioDestino.Nome}";
            descricaoDestino = $"Transferência de {meioOrigem.Nome}";
        }
        else
        {
            descricaoOrigem = $"Pagamento Fatura {meioDestino.Nome}";
            descricaoDestino = $"Pagamento Fatura via {meioOrigem.Nome}";
        }

        var lancamentoOrigem = new Lancamento
        {
            IdUsuario = userInfo.Id,
            IdMeioPagamento = dto.IdMeioPagamentoOrigem,
            Tipo = TipoLancamento.Despesa,
            Operacao = OperacaoLancamento.Transferencia,
            TipoTransferencia = tipoTransferencia,
            Data = dto.Data,
            Descricao = descricaoOrigem,

            Valor = dto.Valor * -1,  //despesa, tem que tornar negativo

            Realizado = true
        };

        var lancamentoDestino = new Lancamento
        {
            IdUsuario = userInfo.Id,
            IdMeioPagamento = dto.IdMeioPagamentoDestino,
            Tipo = TipoLancamento.Receita,
            Operacao = OperacaoLancamento.Transferencia,
            TipoTransferencia = tipoTransferencia,
            Data = dto.Data,
            Descricao = descricaoDestino,

            Valor = dto.Valor,

            Realizado = true
        };

        // transação

        try
        {
            await _lancamentoRepo.BeginTransaction();

            //persiste os lançamentos no banco

            await _lancamentoRepo.Insert(lancamentoOrigem);
            await _lancamentoRepo.Insert(lancamentoDestino);

            //pega os ids, salva as referencias e atualiza

            lancamentoOrigem.IdLancamentoTransferencia = lancamentoDestino.Id;
            lancamentoDestino.IdLancamentoTransferencia = lancamentoOrigem.Id;

            await _lancamentoRepo.Update(lancamentoOrigem);
            await _lancamentoRepo.Update(lancamentoDestino);

            //atualiza o saldo das contas   

            await RecalcularSaldoConta(meioOrigem, lancamentoOrigem.Valor);
            await RecalcularSaldoConta(meioDestino, lancamentoDestino.Valor);

            await _lancamentoRepo.CommitTransaction();

            return lancamentoOrigem.Id;
        }
        catch (Exception)
        {
            await _lancamentoRepo.RollbackTransaction();
            throw;
        }

    }

    private async Task<decimal?> RecalcularSaldoConta(MeioPagamento meio, decimal valorDelta)
    {
        decimal? novoSaldo = null!;

        if (meio.Tipo != TipoMeioPagamento.CartaoCredito)
        {
            meio.Saldo = meio.Saldo + valorDelta;

            await _meioPagamentoRepo.Update(meio);

            novoSaldo = meio.Saldo!.Value;
        }

        return novoSaldo;
    }



    public async Task<Result<TransferenciaUpdateDto>> Obter(int idLancamentoOrigem)
    {
        var userInfo = _authenticationManager.ObterInfoUsuarioLogado();

        var lancamentoOrigem = await _lancamentoRepo.GetById(idLancamentoOrigem);

        var lancamentoDestino = await _lancamentoRepo.GetById(lancamentoOrigem.IdLancamentoTransferencia!.Value);

        var dto = new TransferenciaUpdateDto(lancamentoOrigem, lancamentoDestino);

        return dto;
    }

    public Task<Result<int>> Alterar(TransferenciaUpdateDto dto)
    {
        throw new BusinessException(ErrorCodes.SystemError, "Alteração de Transferência ainda não implementada.");
    }

    public async Task<Result<int>> Excluir(int idLancamentoOrigem)
    {
        var userInfo = _authenticationManager.ObterInfoUsuarioLogado();

        // validações lançamento origem

        var lancamentoOrigem = await _lancamentoRepo.GetById(idLancamentoOrigem);

        if (lancamentoOrigem == null)
            throw new BusinessException(
                code: ErrorCodes.Lancamento_NaoEncontrado,
                message: "Lançamento origem não encontrado");

        if (lancamentoOrigem.IdUsuario != userInfo.Id)
            throw new BusinessException(
                code: ErrorCodes.Lancamento_NaoPertenceAoUsuario,
                message: "Lançamento origem não pertence ao usuário");

        if (lancamentoOrigem.Operacao != OperacaoLancamento.Transferencia)
            throw new BusinessException(
                code: ErrorCodes.Lancamento_OperacaoNaoEhTransferencia,
                message: "Lançamento origem não é uma transferência / pagamento de fatura");

        // validações lançamento destino

        var lancamentoDestino = await _lancamentoRepo.GetById(lancamentoOrigem.IdLancamentoTransferencia!.Value);

        if (lancamentoDestino == null)
            throw new BusinessException(
                code: ErrorCodes.Lancamento_NaoEncontrado,
                message: "Lançamento destino não encontrado");

        if (lancamentoDestino.IdUsuario != userInfo.Id)
            throw new BusinessException(
                code: ErrorCodes.Lancamento_NaoPertenceAoUsuario,
                message: "Lançamento destino não pertence ao usuário");

        if (lancamentoDestino.Operacao != OperacaoLancamento.Transferencia)
            throw new BusinessException(
                code: ErrorCodes.Lancamento_OperacaoNaoEhTransferencia,
                message: "Lançamento destino não é uma transferência / pagamento de fatura");

        // validações meio origem

        var meioOrigem = await _meioPagamentoRepo.GetById(lancamentoOrigem.IdMeioPagamento);

        if (meioOrigem == null)
            throw new BusinessException(
                code: ErrorCodes.MeioPagamento_NaoEncontrado,
                message: "Conta ou Cartão não encontrado");

        if (meioOrigem.IdUsuario != userInfo.Id)
            throw new BusinessException(
                code: ErrorCodes.MeioPagamento_NaoPertenceAoUsuario,
                message: "Conta/Cartão não pertence ao usuário.");

        // validações meio destino

        var meioDestino = await _meioPagamentoRepo.GetById(lancamentoDestino.IdMeioPagamento);

        if (meioDestino == null)
            throw new BusinessException(
                code: ErrorCodes.MeioPagamento_NaoEncontrado,
                message: "Conta ou Cartão não encontrado");

        if (meioDestino.IdUsuario != userInfo.Id)
            throw new BusinessException(
                code: ErrorCodes.MeioPagamento_NaoPertenceAoUsuario,
                message: "Conta/Cartão não pertence ao usuário.");


        // transação 

        try
        {
            await _lancamentoRepo.BeginTransaction();

            // atualiza saldo origem e destino (somente se não for cartao)
            await RecalcularSaldoConta(meioOrigem, (lancamentoOrigem.Valor * -1)); // inverte o sinal pra reverter o valor do saldo
            await RecalcularSaldoConta(meioDestino, (lancamentoDestino.Valor * -1)); // inverte o sinal pra reverter o valor do saldo

            // tira a referencia do origem, pra poder apagar depois
            lancamentoOrigem.IdLancamentoTransferencia = null;
            await _lancamentoRepo.Update(lancamentoOrigem);

            await _lancamentoRepo.Delete(lancamentoDestino.Id);
            await _lancamentoRepo.Delete(lancamentoOrigem.Id);

            await _lancamentoRepo.CommitTransaction();

            return (idLancamentoOrigem, 
                $"{lancamentoOrigem.TipoTransferencia!.Value.ObterDescricao()} excluída com sucesso.");
        }
        catch (Exception)
        {
            await _lancamentoRepo.RollbackTransaction();
            throw;
        }
    }
}
