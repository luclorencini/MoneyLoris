using MoneyLoris.Application.Business.Lancamentos.Dtos;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Business.Lancamentos;
public interface ITransferenciaService
{
    Task<Result<int>> InserirTransferenciaEntreContas(TransferenciaInsertDto transferencia);
    Task<Result<int>> InserirPagamentoFatura(TransferenciaInsertDto transferencia);
    Task<Result<TransferenciaUpdateDto>> Obter(int id);
    Task<Result<int>> Alterar(TransferenciaUpdateDto lancamento);
    Task<Result<int>> Excluir(int idLancamentoOrigem);
}
