using MoneyLoris.Application.Business.MeiosPagamento.Dtos;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Business.MeiosPagamento;
public interface IMeioPagamentoService
{
    Task<Result<ICollection<MeioPagamentoCadastroListItemDto>>> Listar();

    Task<Result<MeioPagamentoCadastroDto>> Obter(int id);
    Task<Result<int>> Inserir(MeioPagamentoCriacaoDto model);
    Task<Result<int>> Alterar(MeioPagamentoCadastroDto model);
    Task<Result<int>> Excluir(int id);

    Task<Result> Inativar(int id);
    Task<Result> Reativar(int id);
}
