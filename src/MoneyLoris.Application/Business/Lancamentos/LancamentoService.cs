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
    private readonly IMeioPagamentoService _meioPagamentoService;
    private readonly ICategoriaValidator _categoriaValidator;
    private readonly ICategoriaRepository _categoriaRepo;
    private readonly ISubcategoriaRepository _subcategoriaRepository;
    private readonly IAuthenticationManager _authenticationManager;
    private readonly ILancamentoConverter _lancamentoConverter;
    private readonly IParcelaCalculator _parcelaCalculator;

    public LancamentoService(
        ILancamentoValidator lancamentoValidator,
        ILancamentoRepository lancamentoRepo,
        IMeioPagamentoValidator meioPagamentoValidator,
        IMeioPagamentoRepository meioPagamentoRepo,
        IMeioPagamentoService meioPagamentoService,
        ICategoriaValidator categoriaValidator,
        ICategoriaRepository categoriaRepository,
        ISubcategoriaRepository subcategoriaRepo,
        IAuthenticationManager authenticationManager,
        ILancamentoConverter lancamentoConverter,
        IParcelaCalculator parcelaCalculator)
    {
        _lancamentoValidator = lancamentoValidator;
        _lancamentoRepo = lancamentoRepo;
        _meioPagamentoValidator = meioPagamentoValidator;
        _meioPagamentoRepo = meioPagamentoRepo;
        _meioPagamentoService = meioPagamentoService;
        _categoriaValidator = categoriaValidator;
        _categoriaRepo = categoriaRepository;
        _subcategoriaRepository = subcategoriaRepo;
        _authenticationManager = authenticationManager;
        _lancamentoConverter = lancamentoConverter;
        _parcelaCalculator = parcelaCalculator;
    }


    public async Task<Result<LancamentoInfoDto>> Obter(int id)
    {
        var lancamento = await _lancamentoRepo.GetById(id);

        _lancamentoValidator.Existe(lancamento);
        _lancamentoValidator.PertenceAoUsuario(lancamento);

        var dto = new LancamentoInfoDto(lancamento);

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
        //validações

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

        //preparar lançamentos

        ICollection<Lancamento> lancamentos = null!;

        //se for despesa no cartão de crédito com mais de uma parcela, prepara o parcelamento
        if (tipo == TipoLancamento.Despesa && meio.Tipo == TipoMeioPagamento.CartaoCredito && dto.Parcelas > 1)
        {
            lancamentos = PrepararLancamentosParcelados(dto, tipo);
        }
        else
        {
            var lanc = _lancamentoConverter.Converter(dto, tipo);
            lancamentos = new List<Lancamento> { lanc };
        }


        //inicia transação

        decimal? novoSaldo = null!;

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
            novoSaldo = await _meioPagamentoService.RecalcularSaldo(meio, valorTotal);

            await _lancamentoRepo.CommitTransaction();
        }
        catch (Exception)
        {
            await _lancamentoRepo.RollbackTransaction();
            throw;
        }


        //retorna o id do lançamento criado, ou do primeiro lançamento em caso de parcelamento
        return (lancamentos.First().Id, novoSaldo);
    }

    private ICollection<Lancamento> PrepararLancamentosParcelados(LancamentoCadastroDto dto, TipoLancamento tipo)
    {
        var lancamentos = new List<Lancamento>();

        var parcelas = _parcelaCalculator.CalculaParcelas(dto.Valor, dto.Parcelas!.Value, dto.Data);

        short index = 1;

        foreach (var p in parcelas)
        {
            var lanc = _lancamentoConverter.Converter(dto, tipo,
                data: p.data, valor: p.valor,
                descricao: dto.Descricao,
                parcelaAtual: index,
                parcelaTotal: (short?)parcelas.Count
            );

            index++;

            lancamentos.Add(lanc);
        }

        return lancamentos;
    }


    public async Task<Result<int>> Alterar(LancamentoEdicaoDto dto)
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
        
        lancamento.ParcelaAtual = dto.ParcelaAtual;
        lancamento.ParcelaTotal= dto.ParcelaTotal;

        //TODO - futuro: permitir alterar a conta selecionada, e recalcular o saldo de ambas as contas (a antiga e a nova)


        //SALDO: se o valor do lançamento mudar, recalcula o saldo baseado na diferença (exceto para cartões de crédito)

        var valorAtual = lancamento.Valor;
        var novoValor = _lancamentoConverter.AjustaValorLancamento(lancamento.Tipo, dto.Valor);
        lancamento.Valor = novoValor;

        _lancamentoValidator.EstaConsistente(lancamento);

        var delta = novoValor - valorAtual;

        //inicia transação

        decimal? novoSaldo = null!;

        try
        {
            await _lancamentoRepo.BeginTransaction();

            await _lancamentoRepo.Update(lancamento);

            novoSaldo = await _meioPagamentoService.RecalcularSaldo(meio, delta);

            await _lancamentoRepo.CommitTransaction();
        }
        catch (Exception)
        {
            await _lancamentoRepo.RollbackTransaction();
            throw;
        }

        //retorno

        var msg = $"{lancamento.Tipo.ObterDescricao()} lançada com sucesso.";
        if (novoSaldo.HasValue)
            msg += $" Novo saldo: {novoSaldo}";

        return (lancamento.Id, msg);
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

        decimal? novoSaldo = null!;

        try
        {
            await _lancamentoRepo.BeginTransaction();

            await _lancamentoRepo.Delete(idLancamento);

            //só atualiza saldo se não for cartao
            novoSaldo = await _meioPagamentoService.RecalcularSaldo(meio, (lancamento.Valor * -1)); ///inverte o sinal pra reverter o valor do saldo

            await _lancamentoRepo.CommitTransaction();
        }
        catch (Exception)
        {
            await _lancamentoRepo.RollbackTransaction();
            throw;
        }

        //retorno

        var msg = "Lançamento excluído com sucesso.";
        if (novoSaldo.HasValue)
            msg += $" Novo saldo: {novoSaldo}";

        return (idLancamento, msg);
    }
}
