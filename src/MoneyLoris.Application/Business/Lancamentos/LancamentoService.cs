using MoneyLoris.Application.Business.Auth.Interfaces;
using MoneyLoris.Application.Business.Categorias.Interfaces;
using MoneyLoris.Application.Business.Lancamentos.Dtos;
using MoneyLoris.Application.Business.Lancamentos.Interfaces;
using MoneyLoris.Application.Business.MeiosPagamento.Interfaces;
using MoneyLoris.Application.Common.Base;
using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Shared;
using MoneyLoris.Application.Utils;

namespace MoneyLoris.Application.Business.Lancamentos;
public class LancamentoService : ServiceBase, ILancamentoService
{
    private readonly ILancamentoValidator _lancamentoValidator;
    private readonly ILancamentoRepository _lancamentoRepo;
    private readonly IMeioPagamentoValidator _meioPagamentoValidator;
    private readonly IMeioPagamentoRepository _meioPagamentoRepo;
    private readonly ICategoriaValidator _categoriaValidator;
    private readonly ICategoriaRepository _categoriaRepo;
    private readonly ISubcategoriaRepository _subcategoriaRepository;
    private readonly IAuthenticationManager _authenticationManager;
    private readonly IParcelaCalculator _parcelaCalculator;

    public LancamentoService(
        ILancamentoValidator lancamentoValidator,
        ILancamentoRepository lancamentoRepo,
        IMeioPagamentoValidator meioPagamentoValidator,
        IMeioPagamentoRepository meioPagamentoRepo,
        ICategoriaValidator categoriaValidator,
        ICategoriaRepository categoriaRepository,
        ISubcategoriaRepository subcategoriaRepo,
        IAuthenticationManager authenticationManager,
        IParcelaCalculator parcelaCalculator)
    {
        _lancamentoValidator = lancamentoValidator;
        _lancamentoRepo = lancamentoRepo;
        _meioPagamentoValidator = meioPagamentoValidator;
        _meioPagamentoRepo = meioPagamentoRepo;
        _categoriaValidator = categoriaValidator;
        _categoriaRepo = categoriaRepository;
        _subcategoriaRepository = subcategoriaRepo;
        _authenticationManager = authenticationManager;
        _parcelaCalculator = parcelaCalculator;
    }


    public async Task<Result<LancamentoCadastroDto>> Obter(int id)
    {
        var lancamento = await _lancamentoRepo.GetById(id);

        _lancamentoValidator.Existe(lancamento);
        _lancamentoValidator.PertenceAoUsuario(lancamento);

        var dto = new LancamentoCadastroDto(lancamento);

        return dto;
    }

    public async Task<Result<int>> InserirDespesa(LancamentoCadastroDto dto)
    {
        var (idLancamento, novoSaldo) = await Inserir(dto, TipoLancamento.Despesa);

        var msg = "Despesa lançada com sucesso.";
        if (novoSaldo.HasValue)
            msg += $" Novo saldo: {novoSaldo}";

        return (idLancamento, msg);
    }

    public async Task<Result<int>> InserirReceita(LancamentoCadastroDto dto)
    {
        var (idLancamento, novoSaldo) = await Inserir(dto, TipoLancamento.Receita);

        var msg = "Receita lançada com sucesso.";
        if (novoSaldo.HasValue)
            msg += $" Novo saldo: {novoSaldo}";

        return (idLancamento, msg);
    }

    internal async Task<(int, decimal?)> Inserir(LancamentoCadastroDto dto, TipoLancamento tipo)
    {
        _lancamentoValidator.NaoEhAdmin();

        var meio = await _meioPagamentoRepo.GetById(dto.IdMeioPagamento);

        _meioPagamentoValidator.Existe(meio);
        _meioPagamentoValidator.PertenceAoUsuario(meio);
        _meioPagamentoValidator.Ativo(meio);

        var categoria = await _categoriaRepo.GetById(dto.IdCategoria);

        _categoriaValidator.Existe(categoria);
        _categoriaValidator.PertenceAoUsuario(categoria);
        _lancamentoValidator.TipoLancamentoIgualTipoCategoria(tipo, categoria);

        _lancamentoValidator.LancamentoCartaoCreditoTemQueTerParcela(meio, dto.Parcelas);

        var lancamentos = preparaLancamentos(dto, meio, tipo);

        //inicia transação
        try
        {
            decimal valorTotal = 0;

            await _lancamentoRepo.BeginTransaction();

            foreach (var l in lancamentos)
            {
                await _lancamentoRepo.Insert(l);
                valorTotal += l.Valor;
            }

            //só atualiza saldo se não for cartao
            var novoSaldo = await RecalcularSaldoConta(meio, valorTotal);

            await _lancamentoRepo.CommitTransaction();

            return (lancamentos.First().Id, novoSaldo);
        }
        catch (Exception)
        {
            await _lancamentoRepo.RollbackTransaction();
            throw;
        }
    }

    private ICollection<Lancamento> preparaLancamentos(LancamentoCadastroDto dto, MeioPagamento meio, TipoLancamento tipo)
    {
        var lancamentos = new List<Lancamento>();

        //se for despesa no cartão de crédito com mais de uma parcela, prepara o parcelamento
        if (dto.Tipo == TipoLancamento.Despesa && meio.Tipo == TipoMeioPagamento.CartaoCredito && dto.Parcelas > 1)
        {
            var parcelas = _parcelaCalculator.CalculaParcelas(dto.Valor, dto.Parcelas.Value, dto.Data);

            int index = 1;

            foreach (var p in parcelas)
            {
                var lanc = montaLancamentoInclusao(dto, tipo,
                    data: p.data, valor: p.valor,
                    descricao: $"{dto.Descricao} - {index}/{parcelas.Count}"
                );

                index++;

                lancamentos.Add(lanc);
            }
        }
        else
        {
            var lanc = montaLancamentoInclusao(dto, tipo);
            lancamentos.Add(lanc);
        }

        return lancamentos;
    }

    private Lancamento montaLancamentoInclusao(LancamentoCadastroDto dto, TipoLancamento tipo,
        DateTime? data = null, string descricao = null!, decimal? valor = null)
    {

        var userInfo = _authenticationManager.ObterInfoUsuarioLogado();

        var val = (valor.HasValue ? valor.Value : dto.Valor);

        var lancamento = new Lancamento
        {
            IdUsuario = userInfo.Id,

            IdMeioPagamento = dto.IdMeioPagamento,
            IdCategoria = dto.IdCategoria,
            IdSubcategoria = dto.IdSubcategoria,

            Tipo = tipo,

            Data = (data.HasValue ? data.Value : dto.Data),
            Descricao = (!String.IsNullOrWhiteSpace(descricao) ? descricao : dto.Descricao),

            //dto sempre manda o valor positivo. Assim, se for despesa, precisa tornar negativo

            Valor = ValorNegativoSeDespesa(tipo, val),

            Operacao = OperacaoLancamento.LancamentoSimples,
            TipoTransferencia = null,

            Realizado = true,
            IdLancamentoTransferencia = null,
        };

        _lancamentoValidator.EstaConsistente(lancamento);

        return lancamento;
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

    private decimal ValorNegativoSeDespesa(TipoLancamento tipo, decimal valor)
    {
        //troca o sinal do valor se for despesa
        valor = (tipo == TipoLancamento.Despesa ? valor * -1 : valor);
        return valor;
    }


    public async Task<Result<int>> Alterar(LancamentoCadastroDto dto)
    {
        _lancamentoValidator.NaoEhAdmin();

        var lancamento = await _lancamentoRepo.GetById(dto.Id);

        _lancamentoValidator.Existe(lancamento);
        _lancamentoValidator.PertenceAoUsuario(lancamento);

        var meio = await _meioPagamentoRepo.GetById(dto.IdMeioPagamento);

        _meioPagamentoValidator.Existe(meio);
        _meioPagamentoValidator.PertenceAoUsuario(meio);

        _lancamentoValidator.NaoPodeTrocarMeioPagamento(lancamento, dto.IdMeioPagamento);

        var categoria = await _categoriaRepo.GetById(dto.IdCategoria);

        _categoriaValidator.Existe(categoria);
        _categoriaValidator.PertenceAoUsuario(categoria);

        if (dto.IdSubcategoria != null)
        {
            var subcat = await _subcategoriaRepository.GetById(dto.IdSubcategoria.Value);

            _categoriaValidator.Existe(subcat);
            _categoriaValidator.PertenceACategoria(subcat, categoria);
        }

        //prepara alteração

        lancamento.Descricao = dto.Descricao;
        lancamento.Data = dto.Data;
        lancamento.IdCategoria = dto.IdCategoria;
        lancamento.IdSubcategoria = dto.IdSubcategoria;

        //TODO - futuro: permitir alterar a conta selecionada, e recalcular o saldo de ambas as contas (a antiga e a nova)


        //SALDO: se o valor do lançamento mudar, recalcula o saldo baseado na diferença (exceto para cartões de crédito)

        var valorAtual = lancamento.Valor;
        var novoValor = ValorNegativoSeDespesa(lancamento.Tipo, dto.Valor);
        var diferenca = valorAtual - novoValor;

        var atualizarSaldo = false;

        if (diferenca != 0)
        {
            lancamento.Valor = novoValor;

            if (meio.Tipo != TipoMeioPagamento.CartaoCredito)
            {
                meio.Saldo -= diferenca;
                atualizarSaldo = true;
            }
        }

        _lancamentoValidator.EstaConsistente(lancamento);

        //inicia transação
        try
        {
            await _lancamentoRepo.BeginTransaction();

            await _lancamentoRepo.Update(lancamento);

            if (atualizarSaldo)
            {
                await _meioPagamentoRepo.Update(meio);
            }

            await _lancamentoRepo.CommitTransaction();
        }
        catch (Exception)
        {
            await _lancamentoRepo.RollbackTransaction();
            throw;
        }

        return (lancamento.Id,
            $"{lancamento.Tipo.ObterDescricao()} lançada com sucesso.");
    }

    public async Task<Result<int>> Excluir(int idLancamento)
    {
        var userInfo = _authenticationManager.ObterInfoUsuarioLogado();

        _lancamentoValidator.NaoEhAdmin();

        var lancamento = await _lancamentoRepo.GetById(idLancamento);

        _lancamentoValidator.Existe(lancamento);
        _lancamentoValidator.PertenceAoUsuario(lancamento);

        var meio = await _meioPagamentoRepo.GetById(lancamento.IdMeioPagamento);

        _meioPagamentoValidator.Existe(meio);
        _meioPagamentoValidator.PertenceAoUsuario(meio);


        //inicia transação
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
}
