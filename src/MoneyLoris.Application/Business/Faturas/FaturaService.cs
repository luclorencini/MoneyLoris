using MoneyLoris.Application.Business.Faturas.Dtos;
using MoneyLoris.Application.Business.Faturas.Interfaces;
using MoneyLoris.Application.Business.Lancamentos.Dtos;
using MoneyLoris.Application.Common.Base;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Business.Faturas;
public class FaturaService : ServiceBase, IFaturaService
{
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
}
