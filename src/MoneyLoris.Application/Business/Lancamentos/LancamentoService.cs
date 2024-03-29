﻿using MoneyLoris.Application.Business.Auth.Interfaces;
using MoneyLoris.Application.Business.Categorias.Interfaces;
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
    private readonly ILancamentoConverter _lancamentoConverter;
    private readonly IParcelaCalculator _parcelaCalculator;
    private readonly IFaturaHelper _faturaHelper;

    public LancamentoService(
        ILancamentoValidator lancamentoValidator,
        ILancamentoRepository lancamentoRepo,
        IMeioPagamentoValidator meioPagamentoValidator,
        IMeioPagamentoRepository meioPagamentoRepo,
        ICategoriaValidator categoriaValidator,
        ICategoriaRepository categoriaRepository,
        ISubcategoriaRepository subcategoriaRepo,
        IAuthenticationManager authenticationManager,
        ILancamentoConverter lancamentoConverter,
        IParcelaCalculator parcelaCalculator,
        IFaturaHelper faturaHelper
    )
    {
        _lancamentoValidator = lancamentoValidator;
        _lancamentoRepo = lancamentoRepo;
        _meioPagamentoValidator = meioPagamentoValidator;
        _meioPagamentoRepo = meioPagamentoRepo;
        _categoriaValidator = categoriaValidator;
        _categoriaRepo = categoriaRepository;
        _subcategoriaRepository = subcategoriaRepo;
        _authenticationManager = authenticationManager;
        _lancamentoConverter = lancamentoConverter;
        _parcelaCalculator = parcelaCalculator;
        _faturaHelper = faturaHelper;
    }


    public async Task<Result<LancamentoInfoDto>> Obter(int id)
    {
        var lancamento = await _lancamentoRepo.ObterInfo(id);

        _lancamentoValidator.Existe(lancamento);
        _lancamentoValidator.PertenceAoUsuario(lancamento);

        var dto = new LancamentoInfoDto(lancamento);

        return dto;
    }

    public async Task<Result<int>> InserirDespesa(LancamentoCadastroDto dto)
    {
        var idLancamento = await Inserir(dto, TipoLancamento.Despesa);
        return (idLancamento, "Despesa lançada com sucesso.");
    }

    public async Task<Result<int>> InserirReceita(LancamentoCadastroDto dto)
    {
        var idLancamento = await Inserir(dto, TipoLancamento.Receita);
        return (idLancamento, "Receita lançada com sucesso.");
    }

    private async Task<int> Inserir(LancamentoCadastroDto dto, TipoLancamento tipo)
    {
        //validações

        _lancamentoValidator.NaoEhAdmin();

        var meio = await _meioPagamentoRepo.GetById(dto.IdMeioPagamento);

        _meioPagamentoValidator.Existe(meio);
        _meioPagamentoValidator.PertenceAoUsuario(meio);
        _meioPagamentoValidator.Ativo(meio);

        _lancamentoValidator.LancamentoCartaoCreditoTemQueTerParcela(meio, dto.Parcelas);
        _lancamentoValidator.LancamentoCartaoCreditoTemQueTerFatura(meio, dto.FaturaMes, dto.FaturaAno);

        var categoria = await _categoriaRepo.GetById(dto.IdCategoria);

        _categoriaValidator.Existe(categoria);
        _categoriaValidator.PertenceAoUsuario(categoria);
        _lancamentoValidator.TipoLancamentoIgualTipoCategoria(tipo, categoria);

        //preparar lançamentos

        ICollection<Lancamento> lancamentos = null!;

        //se for despesa no cartão de crédito com mais de uma parcela, prepara o parcelamento
        if (tipo == TipoLancamento.Despesa && meio.IsCartao() && dto.Parcelas > 1)
        {
            lancamentos = PrepararLancamentosParcelados(dto, tipo);
        }
        else
        {
            var lanc = _lancamentoConverter.Converter(dto, tipo);
            lancamentos = new List<Lancamento> { lanc };
        }

        //preparar faturas para os lançamentos, caso seja cartão de crédito

        if (meio.IsCartao())
        {
            var mesF = dto.FaturaMes!.Value;
            var anoF = dto.FaturaAno!.Value;

            foreach (var l in lancamentos)
            {
                //obtem fatura e seta no lançamento
                var fatura = await _faturaHelper.ObterOuCriarFatura(meio, mesF, anoF);

                l.IdFatura = fatura.Id;

                //define próxima fatura (mês/ano)
                if (mesF == 12)
                {
                    //dezembro: proximo mes é ano novo
                    mesF = 1;
                    anoF++;
                }
                else
                    mesF++; //só incrementa o mês
            }
        }

        //checagem final

        foreach (var l in lancamentos)
        {
            _lancamentoValidator.EstaConsistente(l);
        }


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

            await _lancamentoRepo.CommitTransaction();
        }
        catch (Exception)
        {
            await _lancamentoRepo.RollbackTransaction();
            throw;
        }


        //retorna o id do lançamento criado, ou do primeiro lançamento em caso de parcelamento
        return lancamentos.First().Id;
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

        //_lancamentoValidator.NaoPodeTrocarMeioPagamento(lancamento, dto.IdMeioPagamento);

        //TODO - lorencini - avaliar no futuro se precisará informar fatura na receita de cartão (pagamento de fatura)
        if (lancamento.Tipo == TipoLancamento.Despesa)
        {
            _lancamentoValidator.LancamentoCartaoCreditoTemQueTerFatura(meio, dto.FaturaMes, dto.FaturaAno);
        }

        var categoria = await _categoriaRepo.GetById(dto.IdCategoria);

        _categoriaValidator.Existe(categoria);
        _categoriaValidator.PertenceAoUsuario(categoria);
        _lancamentoValidator.TipoLancamentoIgualTipoCategoria(lancamento.Tipo, categoria);

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
        lancamento.ParcelaTotal = dto.ParcelaTotal;

        //TODO - futuro: permitir alterar a conta selecionada


        //TODO - lorencini - avaliar no futuro se precisará informar fatura na receita de cartão (pagamento de fatura)
        //fatura
        if (meio.IsCartao() && lancamento.Tipo == TipoLancamento.Despesa)
        {
            var fatura = await _faturaHelper.ObterOuCriarFatura(meio, dto.FaturaMes!.Value, dto.FaturaAno!.Value);
            lancamento.IdFatura = fatura.Id;
        }

        //SALDO: se o valor do lançamento mudar, recalcula o saldo baseado na diferença (exceto para cartões de crédito)

        var valorAtual = lancamento.Valor;
        var novoValor = _lancamentoConverter.AjustaValorLancamento(lancamento.Tipo, dto.Valor);
        lancamento.Valor = novoValor;

        _lancamentoValidator.EstaConsistente(lancamento);

        //inicia transação

        try
        {
            await _lancamentoRepo.BeginTransaction();

            await _lancamentoRepo.Update(lancamento);

            await _lancamentoRepo.CommitTransaction();
        }
        catch (Exception)
        {
            await _lancamentoRepo.RollbackTransaction();
            throw;
        }

        //retorno
        return (lancamento.Id, $"{lancamento.Tipo.ObterDescricao()} lançada com sucesso.");
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

            await _lancamentoRepo.CommitTransaction();
        }
        catch (Exception)
        {
            await _lancamentoRepo.RollbackTransaction();
            throw;
        }

        //retorno
        return (idLancamento, "Lançamento excluído com sucesso.");
    }
}
