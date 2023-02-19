using MoneyLoris.Application.Business.MeiosPagamento.Dtos;
using MoneyLoris.Application.Common.Base;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Business.MeiosPagamento;
public class MeioPagamentoService : ServiceBase, IMeioPagamentoService
{
    public Task<Result<ICollection<MeioPagamentoCadastroListItemDto>>> Listar()
    {
        //TODO
        throw new NotImplementedException();
    }

    public Task<Result<MeioPagamentoCadastroDto>> Obter(int id)
    {
        //TODO
        throw new NotImplementedException();
    }

    public Task<Result<int>> Inserir(MeioPagamentoCriacaoDto model)
    {
        //TODO
        throw new NotImplementedException();
    }

    public Task<Result<int>> Alterar(MeioPagamentoCadastroDto model)
    {
        //TODO
        throw new NotImplementedException();
    }

    public Task<Result<int>> Excluir(int id)
    {
        //TODO
        throw new NotImplementedException();
    }

    public Task<Result> Inativar(int id)
    {
        //TODO
        throw new NotImplementedException();
    }

    public Task<Result> Reativar(int id)
    {
        //TODO
        throw new NotImplementedException();
    }


    public Task<Result<ICollection<MeioPagamentoListItemDto>>> ObterMeiosPagamento()
    {
        //TODO
        throw new NotImplementedException();
    }

    public Task<Result<ICollection<MeioPagamentoListItemDto>>> ObterContas()
    {
        //TODO
        throw new NotImplementedException();
    }

    public Task<Result<ICollection<MeioPagamentoListItemDto>>> ObterCartoes()
    {
        //TODO
        throw new NotImplementedException();
    }
}
