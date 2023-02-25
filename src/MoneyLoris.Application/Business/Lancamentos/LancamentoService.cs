using MoneyLoris.Application.Business.Auth.Interfaces;
using MoneyLoris.Application.Business.Lancamentos.Dtos;
using MoneyLoris.Application.Business.MeiosPagamento;
using MoneyLoris.Application.Common.Base;
using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Shared;

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

        var lancamento = PreparaLancamentoSimples(dto, TipoLancamento.Despesa);


        await _lancamentoRepo.BeginTransaction();

        try
        {
            decimal? novoSaldo = null!;

            await _lancamentoRepo.Insert(lancamento);

            //só atualiza saldo se não for cartao
            if (meio.Tipo != TipoMeioPagamento.CartaoCredito)
            {
                meio.Saldo = meio.Saldo + lancamento.Valor;

                await _meioPagamentoRepo.Update(meio);
            }

            await _lancamentoRepo.CommitTransaction();

            return (lancamento.Id, novoSaldo);
        }
        catch (Exception)
        {
            await _lancamentoRepo.RollbackTransaction();
            throw;
        }
    }

    private Lancamento PreparaLancamentoSimples(LancamentoCadastroDto dto, TipoLancamento tipo)
    {
        //TODO - regras de validação

        var userInfo = _authenticationManager.ObterInfoUsuarioLogado();

        var lancamento = new Lancamento
        {
            IdMeioPagamento = dto.IdMeioPagamento,
            IdCategoria = dto.IdCategoria,
            IdSubcategoria = dto.IdSubcategoria,

            Tipo = tipo,

            Data = dto.Data,
            Descricao = dto.Descricao,

            //despesa sempre é negativo
            Valor = (tipo == TipoLancamento.Receita ? dto.Valor : dto.Valor * -1),

            Operacao = OperacaoLancamento.LancamentoSimples,
            TipoTransferencia = null,

            Realizado = true,
            IdLancamentoTransferencia = null,
        };

        return lancamento;
    }

    public async Task<Result<LancamentoCadastroDto>> Obter(int id)
    {
        var lancamento = await obterLancamento(id);

        var dto = new LancamentoCadastroDto(lancamento);

        return dto;
    }

    public Task<Result<int>> Alterar(LancamentoCadastroDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<Result<int>> Excluir(int id)
    {
        throw new NotImplementedException();
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
