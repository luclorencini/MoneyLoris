using MoneyLoris.Application.Business.Lancamentos.Dtos;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Business.Lancamentos.Interfaces;
public interface ILancamentoService
{
    Task<Result<LancamentoInfoDto>> Obter(int id);
    Task<Result<int>> InserirReceita(LancamentoCadastroDto dto);
    Task<Result<int>> InserirDespesa(LancamentoCadastroDto dto);
    Task<Result<int>> Alterar(LancamentoEdicaoDto dto);
    Task<Result<int>> Excluir(int id);
}
