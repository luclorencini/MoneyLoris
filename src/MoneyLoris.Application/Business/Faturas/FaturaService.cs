using MoneyLoris.Application.Business.Auth.Interfaces;
using MoneyLoris.Application.Business.Faturas.Dtos;
using MoneyLoris.Application.Business.Faturas.Interfaces;
using MoneyLoris.Application.Business.Lancamentos.Dtos;
using MoneyLoris.Application.Business.MeiosPagamento.Interfaces;
using MoneyLoris.Application.Common.Base;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Business.Faturas;
public class FaturaService : ServiceBase, IFaturaService
{
    private readonly IFaturaHelper _faturaHelper;
    private readonly IFaturaRepository _faturaRepo;
    private readonly IMeioPagamentoValidator _meioPagamentoValidator;
    private readonly IMeioPagamentoRepository _meioPagamentoRepo;
    private readonly IAuthenticationManager _authenticationManager;

    public FaturaService(
        IFaturaHelper faturaHelper,
        IFaturaRepository faturaRepo,
        IMeioPagamentoRepository meioPagamentoRepo,
        IMeioPagamentoValidator meioPagamentoValidator,
        IAuthenticationManager authenticationManager
    )
    {
        _faturaHelper = faturaHelper;
        _faturaRepo = faturaRepo;
        _meioPagamentoValidator = meioPagamentoValidator;
        _meioPagamentoRepo = meioPagamentoRepo;
        _authenticationManager = authenticationManager;
    }

    public async Task<Result<FaturaInfoDto>> ObterInfo(FaturaAnoMesFiltroDto filtro)
    {
        var userInfo = _authenticationManager.ObterInfoUsuarioLogado();

        var cartao = await _meioPagamentoRepo.GetById(filtro.IdCartao);

        var fatura = await _faturaHelper.ObterOuCriarFatura(cartao, filtro.Mes, filtro.Ano);

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
}
