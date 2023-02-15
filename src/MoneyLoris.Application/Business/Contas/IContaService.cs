using MoneyLoris.Application.Business.Contas.Dtos;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Business.Contas;
public interface IContaService
{
    Task<Result<ICollection<ContaCadastroListItemDto>>> Listar();

    Task<Result<ContaCadastroDto>> Obter(int id);
    Task<Result<int>> Inserir(ContaCriacaoDto model);
    Task<Result<int>> Alterar(ContaCadastroDto model);
    Task<Result<int>> Excluir(int id);

    Task<Result> Inativar(int id);
    Task<Result> Reativar(int id);
}
