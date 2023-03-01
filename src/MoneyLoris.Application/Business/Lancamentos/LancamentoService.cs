using MoneyLoris.Application.Business.Auth.Interfaces;
using MoneyLoris.Application.Business.Lancamentos.Dtos;
using MoneyLoris.Application.Business.MeiosPagamento;
using MoneyLoris.Application.Common.Base;
using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Shared;
using MoneyLoris.Application.Utils;

namespace MoneyLoris.Application.Business.Lancamentos;
public class LancamentoService : ServiceBase, ILancamentoService
{
    private readonly ILancamentoRepository _lancamentoRepo;
    private readonly IMeioPagamentoRepository _meioPagamentoRepo;
    private readonly IAuthenticationManager _authenticationManager;

    public LancamentoService(
        ILancamentoRepository lancamentoRepo,
        IMeioPagamentoRepository meioPagamentoRepo,
        IAuthenticationManager authenticationManager)
    {
        _lancamentoRepo = lancamentoRepo;
        _meioPagamentoRepo = meioPagamentoRepo;
        _authenticationManager = authenticationManager;
    }


    public async Task<Result<Pagination<ICollection<LancamentoListItemDto>>>> Pesquisar(LancamentoFiltroDto filtro)
    {
        var userInfo = _authenticationManager.ObterInfoUsuarioLogado();

        //pega o total
        var total = await _lancamentoRepo.PesquisaTotalRegistros(filtro, userInfo.Id);

        //faz a consulta paginada
        var lancamentos = await _lancamentoRepo.PesquisaPaginada(filtro, userInfo.Id);

        //transforma no tipo de retorno
        ICollection<LancamentoListItemDto> ret =
            lancamentos.Select(l => new LancamentoListItemDto(l)).ToList();

        return Pagination(pagedData: ret, total: total);
    }

    public async Task<Result<LancamentoBalancoDto>> ObterBalanco(LancamentoFiltroDto filtro)
    {
        var userInfo = _authenticationManager.ObterInfoUsuarioLogado();

        var receita = await _lancamentoRepo.SomatorioReceitasFiltro(filtro, userInfo.Id);
        var despesa = await _lancamentoRepo.SomatorioDespesasFiltro(filtro, userInfo.Id);
        var balanco = receita + despesa;

        var ret = new LancamentoBalancoDto
        {
            Receitas = receita,
            Despesas = despesa,
            Balanco = balanco
        };

        return ret;
    }


    public async Task<Result<int>> InserirDespesa(LancamentoCadastroDto dto)
    {
        var (idLancamento, novoSaldo) = await InserirLancamentoSimplesTransactional(dto, TipoLancamento.Despesa);

        var msg = "Despesa lançada com sucesso.";
        if (novoSaldo.HasValue)
            msg += $" Novo saldo: {novoSaldo}";

        return (idLancamento, msg);
    }

    public async Task<Result<int>> InserirReceita(LancamentoCadastroDto dto)
    {
        var (idLancamento, novoSaldo) = await InserirLancamentoSimplesTransactional(dto, TipoLancamento.Receita);

        var msg = "Receita lançada com sucesso.";
        if (novoSaldo.HasValue)
            msg += $" Novo saldo: {novoSaldo}";

        return (idLancamento, msg);
    }

    internal async Task<(int, decimal?)> InserirLancamentoSimplesTransactional(LancamentoCadastroDto dto, TipoLancamento tipo)
    {
        var userInfo = _authenticationManager.ObterInfoUsuarioLogado();

        var meio = await _meioPagamentoRepo.GetById(dto.IdMeioPagamento);

        if (meio == null)
            throw new BusinessException(
                code: ErrorCodes.MeioPagamento_NaoEncontrado,
                message: "Conta ou Cartão não encontrado");

        if (meio.IdUsuario != userInfo.Id)
            throw new BusinessException(
                code: ErrorCodes.MeioPagamento_NaoPertenceAoUsuario,
                message: "Conta/Cartão não pertence ao usuário.");

        var lancamento = PreparaLancamentoSimples(dto, tipo, userInfo.Id);

        try
        {
            await _lancamentoRepo.BeginTransaction();

            await _lancamentoRepo.Insert(lancamento);

            //só atualiza saldo se não for cartao
            var novoSaldo = await RecalcularSaldoConta(meio, lancamento.Valor);

            await _lancamentoRepo.CommitTransaction();

            return (lancamento.Id, novoSaldo);
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

    private Lancamento PreparaLancamentoSimples(LancamentoCadastroDto dto, TipoLancamento tipo, int idUsuario)
    {
        //TODO - regras de validação

        var userInfo = _authenticationManager.ObterInfoUsuarioLogado();

        var lancamento = new Lancamento
        {
            IdUsuario = idUsuario,

            IdMeioPagamento = dto.IdMeioPagamento,
            IdCategoria = dto.IdCategoria,
            IdSubcategoria = dto.IdSubcategoria,

            Tipo = tipo,

            Data = dto.Data,
            Descricao = dto.Descricao,

            //dto sempre manda o valor negativo. Assim, se for despesa, precisa tornar negativo
            Valor = ValorNegativoSeDespesa(tipo, dto.Valor),

            Operacao = OperacaoLancamento.LancamentoSimples,
            TipoTransferencia = null,

            Realizado = true,
            IdLancamentoTransferencia = null,
        };

        return lancamento;
    }

    private decimal ValorNegativoSeDespesa(TipoLancamento tipo, decimal valor)
    {
        //troca o sinal do valor se for despesa
        valor = (tipo == TipoLancamento.Despesa ? valor * -1 : valor);
        return valor;
    }




    public async Task<Result<LancamentoCadastroDto>> Obter(int id)
    {
        var lancamento = await obterLancamento(id);

        var dto = new LancamentoCadastroDto(lancamento);

        return dto;
    }

    public async Task<Result<int>> Alterar(LancamentoCadastroDto dto)
    {
        //throw new BusinessException(ErrorCodes.SystemError, "Alteração de Lançamento ainda não implementada.");

        // var userInfo = _authenticationManager.ObterInfoUsuarioLogado();
        // 
        // var meio = await _meioPagamentoRepo.GetById(dto.IdMeioPagamento);
        // 
        // //validação meio pagamento
        // 
        // if (meio == null)
        //     throw new BusinessException(
        //         code: ErrorCodes.MeioPagamento_NaoEncontrado,
        //         message: "Conta ou Cartão não encontrado");
        // 
        // if (meio.IdUsuario != userInfo.Id)
        //     throw new BusinessException(
        //         code: ErrorCodes.MeioPagamento_NaoPertenceAoUsuario,
        //         message: "Conta/Cartão não pertence ao usuário.");
        // 
        // if (meio.Id != dto.IdMeioPagamento)
        //     throw new BusinessException(
        //         code: ErrorCodes.MeioPagamento_TipoDiferenteAlteracao,
        //         message: "Não é possível trocar o meio de pagamento na alteração.");
        // 
        // // validação lançamento
        // 
        // var lancamento = await _lancamentoRepo.GetById(dto.Id);
        // 
        // if (lancamento == null)
        //     throw new BusinessException(
        //         code: ErrorCodes.Lancamento_NaoEncontrado,
        //         message: "Lançamento não encontrado");
        // 
        // if (lancamento.IdUsuario == userInfo.Id)
        //     throw new BusinessException(
        //         code: ErrorCodes.Lancamento_NaoPertenceAoUsuario,
        //         message: "Lançamento não pertence ao usuário");
        // 
        // 
        // //TODO - validação categoria e subcategoria
        // 
        // //prepara alteração
        // 
        // lancamento.Descricao = dto.Descricao;
        // lancamento.Data = dto.Data;
        // lancamento.IdCategoria = dto.IdCategoria;
        // lancamento.IdSubcategoria = dto.IdSubcategoria;
        // 
        // //TODO - futuro: permitir alterar conta, e recalcular o saldo de ambas as contas
        // 
        // //TODO - ajusta o novo valor do lançamento, e recalcula o saldo baseado na diferença
        // 
        // await _lancamentoRepo.Update(lancamento);
        // 
        // 
        // return (lancamento.Id,
        //     $"{lancamento.Tipo.ObterDescricao()} lançada com sucesso.");
    }

    public async Task<Result<int>> Excluir(int idLancamento)
    {
        var userInfo = _authenticationManager.ObterInfoUsuarioLogado();

        //lançamento

        var lancamento = await _lancamentoRepo.GetById(idLancamento);

        if (lancamento == null)
            throw new BusinessException(
                code: ErrorCodes.Lancamento_NaoEncontrado,
                message: "Lançamento não encontrado");

        if (lancamento.IdUsuario != userInfo.Id)
            throw new BusinessException(
                code: ErrorCodes.Lancamento_NaoPertenceAoUsuario,
                message: "Lançamento não pertence ao usuário");

        //meio

        var meio = await _meioPagamentoRepo.GetById(lancamento.IdMeioPagamento);

        if (meio == null)
            throw new BusinessException(
                code: ErrorCodes.MeioPagamento_NaoEncontrado,
                message: "Conta ou Cartão não encontrado");

        if (meio.IdUsuario != userInfo.Id)
            throw new BusinessException(
                code: ErrorCodes.MeioPagamento_NaoPertenceAoUsuario,
                message: "Conta/Cartão não pertence ao usuário.");


        try
        {
            await _lancamentoRepo.BeginTransaction();

            await _lancamentoRepo.Delete(idLancamento);

            //só atualiza saldo se não for cartao
            var novoSaldo = await RecalcularSaldoConta(meio, (lancamento.Valor * -1)); ///inverte o sinal pra reverter o valor do saldo

            await _lancamentoRepo.CommitTransaction();

            var msg = "Lançamento excluído com sucesso.";
            if (novoSaldo.HasValue)
                msg += $" Novo saldo: {novoSaldo}";

            return (idLancamento, msg);
        }
        catch (Exception)
        {
            await _lancamentoRepo.RollbackTransaction();
            throw;
        }
    }

    private async Task<Lancamento> obterLancamento(int id)
    {
        var lancamento = await _lancamentoRepo.GetById(id);

        if (lancamento == null)
            throw new BusinessException(
                code: ErrorCodes.Lancamento_NaoEncontrado,
                message: "Lançamento não encontrado");

        return lancamento;
    }

    public async Task<Result<ICollection<LancamentoSugestaoDto>>> ObterSugestoesDespesas(string termoBusca)
    {
        //TODO - implementar

        ICollection<LancamentoSugestaoDto> list = new List<LancamentoSugestaoDto>();

        return await TaskSuccess(list);
    }

    public async Task<Result<ICollection<LancamentoSugestaoDto>>> ObterSugestoesReceitas(string termoBusca)
    {
        //TODO - implementar

        ICollection<LancamentoSugestaoDto> list = new List<LancamentoSugestaoDto>();

        return await TaskSuccess(list);
    }
}
