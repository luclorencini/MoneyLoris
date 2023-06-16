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
    //private readonly IMeioPagamentoRepository _meioPagamentoRepo;

    public FaturaService(
        IFaturaFactory faturaFactory,
        IFaturaRepository faturaRepo,
        //IMeioPagamentoRepository meioPagamentoRepo,
        IMeioPagamentoValidator meioPagamentoValidator
    )
    {
        _faturaFactory = faturaFactory;
        _faturaRepo = faturaRepo;
        _meioPagamentoValidator = meioPagamentoValidator;
        //_meioPagamentoRepo = meioPagamentoRepo;
    }

    public Task<Result<FaturaInfoDto>> ObterInfo(FaturaFiltroDto filtro)
    {
        //TODO - Lorencini - implementar e escrever testes
        throw new NotImplementedException();
    }

    public Task<Result<Pagination<ICollection<LancamentoListItemDto>>>> Pesquisar(FaturaFiltroDto filtro)
    {
        //TODO - Lorencini - implementar e escrever testes
        throw new NotImplementedException();
    }

    public Task<Result<FaturaSelecaoDto>> ObterFaturaAtual(int IdCartao)
    {
        //TODO - Lorencini - implementar e escrever testes
        throw new NotImplementedException();
    }

    public Task<Result<ICollection<FaturaSelecaoDto>>> ObterFaturasSelecao(int IdCartao)
    {
        //TODO - Lorencini - implementar e escrever testes
        throw new NotImplementedException();
    }

    //public async Task<Fatura> ObterOuCriarFatura(int idCartao, int mes, int ano)
    //{

    //}

    public async Task<Fatura> ObterOuCriarFatura(MeioPagamento cartao, int mes, int ano)
    {
        //_meioPagamentoValidator.Existe(cartao);
        //_meioPagamentoValidator.PertenceAoUsuario(cartao);
        _meioPagamentoValidator.EhCartaoCredito(cartao);
        //_meioPagamentoValidator.Ativo(cartao);

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
