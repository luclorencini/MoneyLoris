using MoneyLoris.Application.Business.Auth.Interfaces;
using MoneyLoris.Application.Business.Faturas;
using MoneyLoris.Application.Business.Faturas.Interfaces;
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
    private readonly IMeioPagamentoRepository _meioPagamentoRepo;
    private readonly ILancamentoValidator _lancamentoValidator;
    private readonly ILancamentoRepository _lancamentoRepo;
    private readonly ITransferenciaValidator _transferenciaValidator;
    private readonly IFaturaHelper _faturaHelper;
    
    private readonly IFaturaRepository _faturaRepo;
    private readonly IAuthenticationManager _authenticationManager;

    public TransferenciaService(
        IMeioPagamentoValidator meioPagamentoValidator,
        IMeioPagamentoRepository meioPagamentoRepo,
        ILancamentoValidator lancamentoValidator,
        ILancamentoRepository lancamentoRepo,
        ITransferenciaValidator transferenciaValidator,
        IFaturaHelper faturaHelper,
        IFaturaRepository faturaRepo,
        IAuthenticationManager authenticationManager
    )
    {
        _meioPagamentoValidator = meioPagamentoValidator;
        _meioPagamentoRepo = meioPagamentoRepo;
        _lancamentoValidator = lancamentoValidator;
        _lancamentoRepo = lancamentoRepo;
        _transferenciaValidator = transferenciaValidator;
        _faturaHelper = faturaHelper;
        _faturaRepo = faturaRepo;
        _authenticationManager = authenticationManager;
    }

    public async Task<Result<int>> InserirTransferenciaEntreContas(TransferenciaInsertDto dto)
    {
        _transferenciaValidator.NaoEhAdmin();

        var userInfo = _authenticationManager.ObterInfoUsuarioLogado();

        var meioOrigem = await _meioPagamentoRepo.GetById(dto.IdMeioPagamentoOrigem);

        _meioPagamentoValidator.OrigemExiste(meioOrigem);
        _meioPagamentoValidator.OrigemPertenceAoUsuario(meioOrigem);
        _transferenciaValidator.MeioOrigemNaoPodeSerCartao(meioOrigem);

        var meioDestino = await _meioPagamentoRepo.GetById(dto.IdMeioPagamentoDestino);

        _meioPagamentoValidator.DestinoExiste(meioDestino);
        _meioPagamentoValidator.DestinoPertenceAoUsuario(meioDestino);
        _transferenciaValidator.SeTransferenciaEntreContasMeioDestinoNaoPodeSerCartao(TipoTransferencia.TransferenciaEntreContas, meioDestino);

        // setup

        var lancamentoOrigem = new Lancamento
        {
            IdUsuario = userInfo.Id,
            IdMeioPagamento = dto.IdMeioPagamentoOrigem,
            Tipo = TipoLancamento.Despesa,
            Operacao = OperacaoLancamento.Transferencia,
            TipoTransferencia = TipoTransferencia.TransferenciaEntreContas,
            Data = dto.Data,
            Descricao = $"Transferência para {meioDestino.Nome}",

            Valor = dto.Valor * -1,  //despesa, tem que tornar negativo

            Realizado = true
        };

        var lancamentoDestino = new Lancamento
        {
            IdUsuario = userInfo.Id,
            IdMeioPagamento = dto.IdMeioPagamentoDestino,
            Tipo = TipoLancamento.Receita,
            Operacao = OperacaoLancamento.Transferencia,
            TipoTransferencia = TipoTransferencia.TransferenciaEntreContas,
            Data = dto.Data,
            Descricao = $"Transferência de {meioOrigem.Nome}",

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

    public async Task<Result<int>> InserirPagamentoFatura(TransferenciaInsertDto dto)
    {
        _transferenciaValidator.NaoEhAdmin();

        var userInfo = _authenticationManager.ObterInfoUsuarioLogado();

        var meioOrigem = await _meioPagamentoRepo.GetById(dto.IdMeioPagamentoOrigem);

        _meioPagamentoValidator.OrigemExiste(meioOrigem);
        _meioPagamentoValidator.OrigemPertenceAoUsuario(meioOrigem);
        _transferenciaValidator.MeioOrigemNaoPodeSerCartao(meioOrigem);

        var meioDestino = await _meioPagamentoRepo.GetById(dto.IdMeioPagamentoDestino);

        _meioPagamentoValidator.DestinoExiste(meioDestino);
        _meioPagamentoValidator.DestinoPertenceAoUsuario(meioDestino);
        _transferenciaValidator.SePagamentoFaturaMeioDestinoTemQueSerCartao(TipoTransferencia.PagamentoFatura, meioDestino);
        _transferenciaValidator.SePagamentoFaturaMesAnoFaturaDevemSerInformados(TipoTransferencia.PagamentoFatura, dto.FaturaMes, dto.FaturaAno);

        // setup

        var lancamentoOrigem = new Lancamento
        {
            IdUsuario = userInfo.Id,
            IdMeioPagamento = dto.IdMeioPagamentoOrigem,
            Tipo = TipoLancamento.Despesa,
            Operacao = OperacaoLancamento.Transferencia,
            TipoTransferencia = TipoTransferencia.PagamentoFatura,
            Data = dto.Data,
            Descricao = $"Pagamento Fatura {meioDestino.Nome}",

            Valor = dto.Valor * -1,  //despesa, tem que tornar negativo

            Realizado = true
        };

        var lancamentoDestino = new Lancamento
        {
            IdUsuario = userInfo.Id,
            IdMeioPagamento = dto.IdMeioPagamentoDestino,
            Tipo = TipoLancamento.Receita,
            Operacao = OperacaoLancamento.Transferencia,
            TipoTransferencia = TipoTransferencia.PagamentoFatura,
            Data = dto.Data,
            Descricao = $"Pagamento Fatura via {meioOrigem.Nome}",

            Valor = dto.Valor,

            Realizado = true
        };

        // transação

        try
        {
            await _lancamentoRepo.BeginTransaction();

            //obtem a fatura informada

            var fatura = await _faturaHelper.ObterOuCriarFatura(meioDestino, dto.FaturaMes!.Value, dto.FaturaAno!.Value);

            //persiste os lançamentos no banco

            await _lancamentoRepo.Insert(lancamentoOrigem);
            await _lancamentoRepo.Insert(lancamentoDestino);

            //pega os ids, salva as referencias e atualiza

            lancamentoOrigem.IdLancamentoTransferencia = lancamentoDestino.Id;
            lancamentoOrigem.IdFatura = fatura.Id;

            lancamentoDestino.IdLancamentoTransferencia = lancamentoOrigem.Id;
            lancamentoDestino.IdFatura = fatura.Id;

            await _lancamentoRepo.Update(lancamentoOrigem);
            await _lancamentoRepo.Update(lancamentoDestino);

            //atualiza o saldo da conta origem

            await RecalcularSaldoConta(meioOrigem, lancamentoOrigem.Valor);

            await _faturaHelper.LancarValorPagoFatura(fatura, dto.Valor);

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
        var lancamentoOrigem = await _lancamentoRepo.GetById(idLancamentoOrigem);

        var lancamentoDestino = await _lancamentoRepo.GetById(lancamentoOrigem.IdLancamentoTransferencia!.Value);

        Fatura? fatura = default!;

        if (lancamentoDestino.IdFatura is not null)
        {
            fatura = await _faturaRepo.GetById(lancamentoDestino.IdFatura.Value);
        }

        var dto = new TransferenciaUpdateDto(lancamentoOrigem, lancamentoDestino, fatura);

        return dto;
    }

    public Task<Result<int>> Alterar(TransferenciaUpdateDto dto)
    {
        throw new BusinessException(ErrorCodes.SystemError, "Alteração de Transferência ainda não implementada.");
    }

    public async Task<Result<int>> Excluir(int idLancamentoOrigem)
    {
        _transferenciaValidator.NaoEhAdmin();

        var lancamentoOrigem = await _lancamentoRepo.GetById(idLancamentoOrigem);

        _lancamentoValidator.OrigemExiste(lancamentoOrigem);
        _lancamentoValidator.OrigemPertenceAoUsuario(lancamentoOrigem);
        _transferenciaValidator.OperacaoLancamentoOrigemTemQueSerTransferencia(lancamentoOrigem);

        var lancamentoDestino = await _lancamentoRepo.GetById(lancamentoOrigem.IdLancamentoTransferencia!.Value);

        _lancamentoValidator.DestinoExiste(lancamentoDestino);
        _lancamentoValidator.DestinoPertenceAoUsuario(lancamentoDestino);
        _transferenciaValidator.OperacaoLancamentoDestinoTemQueSerTransferencia(lancamentoDestino);

        var meioOrigem = await _meioPagamentoRepo.GetById(lancamentoOrigem.IdMeioPagamento);

        _meioPagamentoValidator.OrigemExiste(meioOrigem);
        _meioPagamentoValidator.OrigemPertenceAoUsuario(meioOrigem);
        _transferenciaValidator.MeioOrigemNaoPodeSerCartao(meioOrigem);

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

            //se foi pagamento de fatura (lançamento destino possui fatura associada), diminui o valor excluído do saldo da fatura
            if (lancamentoDestino.IdFatura is not null)
            {
                var fatura = await _faturaRepo.GetById(lancamentoDestino.IdFatura.Value);

                await _faturaHelper.SubtrairValorPagoFatura(fatura, lancamentoDestino.Valor);
            }

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
