using MoneyLoris.Application.Business.Auth.Interfaces;
using MoneyLoris.Application.Business.Faturas.Dtos;
using MoneyLoris.Application.Business.Faturas.Interfaces;
using MoneyLoris.Application.Business.Lancamentos.Dtos;
using MoneyLoris.Application.Business.MeiosPagamento.Interfaces;
using MoneyLoris.Application.Common.Base;
using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Business.Faturas;
public class FaturaService : ServiceBase, IFaturaService
{
    private readonly IFaturaFactory _faturaFactory;
    private readonly IFaturaRepository _faturaRepo;
    private readonly IMeioPagamentoValidator _meioPagamentoValidator;
    private readonly IMeioPagamentoRepository _meioPagamentoRepo;
    private readonly IAuthenticationManager _authenticationManager;

    public FaturaService(
        IFaturaFactory faturaFactory,
        IFaturaRepository faturaRepo,
        IMeioPagamentoRepository meioPagamentoRepo,
        IMeioPagamentoValidator meioPagamentoValidator,
        IAuthenticationManager authenticationManager
    )
    {
        _faturaFactory = faturaFactory;
        _faturaRepo = faturaRepo;
        _meioPagamentoValidator = meioPagamentoValidator;
        _meioPagamentoRepo = meioPagamentoRepo;
        _authenticationManager = authenticationManager;
    }

    public async Task<Result<FaturaInfoDto>> ObterInfo(FaturaAnoMesFiltroDto filtro)
    {
        var userInfo = _authenticationManager.ObterInfoUsuarioLogado();

        var cartao = await _meioPagamentoRepo.GetById(filtro.IdCartao);

        var fatura = await ObterOuCriarFatura(cartao, filtro.Mes, filtro.Ano);

        var dto = new FaturaInfoDto(fatura);

        dto.ValorFatura = await _faturaRepo.ObterValorFatura(fatura.Id, userInfo.Id);

        return dto;
    }

    public async Task<Result<Pagination<ICollection<LancamentoListItemDto>>>> Pesquisar(FaturaFiltroDto filtro)
    {
        var userInfo = _authenticationManager.ObterInfoUsuarioLogado();

        //pega o total
        var total = await _faturaRepo.PesquisaTotalRegistros(filtro, userInfo.Id);

        //faz a consulta paginada
        var lancamentos = await _faturaRepo.PesquisaPaginada(filtro, userInfo.Id);

        //transforma no tipo de retorno
        ICollection<LancamentoListItemDto> ret =
            lancamentos.Select(l => new LancamentoListItemDto(l)).ToList();

        return Pagination(pagedData: ret, total: total);
    }

    public async Task<Result<FaturaSelecaoDto>> ObterFaturaEmAberto(int IdCartao)
    {
        var cartao = await _meioPagamentoRepo.GetById(IdCartao);

        _meioPagamentoValidator.Existe(cartao);
        _meioPagamentoValidator.PertenceAoUsuario(cartao);
        _meioPagamentoValidator.Ativo(cartao);
        _meioPagamentoValidator.EhCartaoCredito(cartao);

        var dataBase = DateTime.Today;

        if (dataBase.Day >= cartao.DiaFechamento)
        {
            //dia do fechamento ou depois: volta fatura do próximo mês
            dataBase = dataBase.AddMonths(1);
        }

        var dto = new FaturaSelecaoDto
        {
            Mes = dataBase.Month,
            Ano = dataBase.Year
        };

        return dto;

    }

    public Task<Result<ICollection<FaturaSelecaoDto>>> ObterFaturasSelecao(int IdCartao)
    {

        //a partir do mês/ano atual, lista os 6 meses anteriores e os 10 posteriores

        ICollection<FaturaSelecaoDto> list = new List<FaturaSelecaoDto>();

        var d = DateTime.Today.AddMonths(-6);

        for (int i = 0; i < 16; i++)
        {
            var f = new FaturaSelecaoDto
            {
                Mes = d.Month,
                Ano = d.Year
            };

            list.Add(f);

            d = d.AddMonths(1);
        }

        return TaskSuccess(list);
    }

    public async Task<Fatura> ObterOuCriarFatura(MeioPagamento cartao, int mes, int ano)
    {
        _meioPagamentoValidator.EhCartaoCredito(cartao);

        //busca fatura pelos campos informados

        var fatura = await _faturaRepo.ObterPorMesAno(cartao.Id, mes, ano);

        if (fatura != null)
            return fatura;

        //se nao encontrou, cria uma nova fatura

        var novaFatura = _faturaFactory.Criar(cartao, mes, ano);

        await _faturaRepo.Insert(novaFatura);

        return novaFatura;

    }
}
