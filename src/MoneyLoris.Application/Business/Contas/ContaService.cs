using MoneyLoris.Application.Business.Contas.Dtos;
using MoneyLoris.Application.Common.Base;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Business.Contas;
public class ContaService : ServiceBase, IContaService
{
    public Task<Result<ICollection<ContaCadastroListItemDto>>> Listar()
    {
        //TODO
        throw new NotImplementedException();
    }

    public Task<Result<ContaCadastroDto>> Obter(int id)
    {
        //TODO
        throw new NotImplementedException();
    }

    public Task<Result<int>> Inserir(ContaCriacaoDto model)
    {
        //TODO
        throw new NotImplementedException();
    }

    public Task<Result<int>> Alterar(ContaCadastroDto model)
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
}
