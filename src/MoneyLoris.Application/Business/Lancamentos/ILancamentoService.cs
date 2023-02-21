using MoneyLoris.Application.Business.Lancamentos.Dtos;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Business.Lancamentos;
public interface ILancamentoService
{
    Task<Result<Pagination<ICollection<LancamentoListItemDto>>>> Pesquisar(LancamentoFiltroDto filtro);
    Task<Result<LancamentoBalancoDto>> ObterBalanco(LancamentoFiltroDto filtro);

    Task<Result<ICollection<LancamentoSugestaoDto>>> ObterSugestoesDespesas(string termoBusca);
    Task<Result<ICollection<LancamentoSugestaoDto>>> ObterSugestoesReceitas(string termoBusca);

    Task<Result<int>> InserirReceita(LancamentoInsertDto lancamento);
    Task<Result<int>> InserirDespesa(LancamentoInsertDto lancamento);
    Task<Result<int>> InserirTransferencia(TransferenciaInsertDto transferencia);
    Task<Result<int>> InserirPagamentoFatura(TransferenciaInsertDto transferencia);
}
