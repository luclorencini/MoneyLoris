using MoneyLoris.Application.Business.Lancamentos.Dtos;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Business.Lancamentos.Interfaces;
public interface ITransferenciaService
{
    Task<Result<int>> InserirTransferenciaEntreContas(TransferenciaInsertDto dto);
    Task<Result<int>> InserirPagamentoFatura(TransferenciaInsertDto dto);
    Task<Result<TransferenciaUpdateDto>> Obter(int id);
    Task<Result<int>> Alterar(TransferenciaUpdateDto dto);
    Task<Result<int>> Excluir(int idLancamentoOrigem);
}
