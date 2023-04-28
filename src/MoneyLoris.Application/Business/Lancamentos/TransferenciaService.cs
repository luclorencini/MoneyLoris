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
    private readonly IMeioPagamentoValidator _meioPagamentoValidator;
    private readonly ILancamentoValidator _lancamentoValidator;
    private readonly ITransferenciaValidator _transferenciaValdator;
    private readonly ILancamentoRepository _lancamentoRepo;
    private readonly IMeioPagamentoRepository _meioPagamentoRepo;
    private readonly IAuthenticationManager _authenticationManager;

    public TransferenciaService(
        IMeioPagamentoValidator meioPagamentoValidator,
        ILancamentoValidator lancamentoValidator,
        ITransferenciaValidator transferenciaValdator,
        ILancamentoRepository lancamentoRepo,
        IMeioPagamentoRepository meioPagamentoRepo,
        IAuthenticationManager authenticationManager)
    {
        _lancamentoValidator = lancamentoValidator;
        _meioPagamentoValidator = meioPagamentoValidator;
        _transferenciaValdator = transferenciaValdator;
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
        _transferenciaValdator.NaoEhAdmin();

        var userInfo = _authenticationManager.ObterInfoUsuarioLogado();

        var meioOrigem = await _meioPagamentoRepo.GetById(dto.IdMeioPagamentoOrigem);

        _meioPagamentoValidator.OrigemExiste(meioOrigem);
        _meioPagamentoValidator.OrigemPertenceAoUsuario(meioOrigem);
        _transferenciaValdator.MeioOrigemNaoPodeSerCartao(meioOrigem);

        var meioDestino = await _meioPagamentoRepo.GetById(dto.IdMeioPagamentoDestino);

        _meioPagamentoValidator.DestinoExiste(meioDestino);
        _meioPagamentoValidator.DestinoPertenceAoUsuario(meioDestino);
        _transferenciaValdator.SeTransferenciaEntreContasMeioDestinoNaoPodeSerCartao(tipoTransferencia, meioDestino);
        _transferenciaValdator.SePagamentoFaturaMeioDestinoTemQueSerCartao(tipoTransferencia, meioDestino);

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
        _transferenciaValdator.NaoEhAdmin();

        var lancamentoOrigem = await _lancamentoRepo.GetById(idLancamentoOrigem);

        _lancamentoValidator.OrigemExiste(lancamentoOrigem);
        _lancamentoValidator.OrigemPertenceAoUsuario(lancamentoOrigem);
        _transferenciaValdator.OperacaoLancamentoOrigemTemQueSerTransferencia(lancamentoOrigem);

        var lancamentoDestino = await _lancamentoRepo.GetById(lancamentoOrigem.IdLancamentoTransferencia!.Value);

        _lancamentoValidator.DestinoExiste(lancamentoDestino);
        _lancamentoValidator.DestinoPertenceAoUsuario(lancamentoDestino);
        _transferenciaValdator.OperacaoLancamentoDestinoTemQueSerTransferencia(lancamentoDestino);

        var meioOrigem = await _meioPagamentoRepo.GetById(lancamentoOrigem.IdMeioPagamento);

        _meioPagamentoValidator.OrigemExiste(meioOrigem);
        _meioPagamentoValidator.OrigemPertenceAoUsuario(meioOrigem);

        var meioDestino = await _meioPagamentoRepo.GetById(lancamentoDestino.IdMeioPagamento);

        _meioPagamentoValidator.DestinoExiste(meioDestino);
        _meioPagamentoValidator.DestinoPertenceAoUsuario(meioDestino);


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
