using MoneyLoris.Application.Business.Lancamentos.Dtos;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Business.Lancamentos;
public interface ILancamentoService
{
    Task<Result<Pagination<ICollection<LancamentoListItemDto>>>> Pesquisar(LancamentoFiltroDto filtro);
    Task<Result<LancamentoBalancoDto>> ObterBalanco(LancamentoFiltroDto filtro);

    Task<Result<ICollection<LancamentoSugestaoDto>>> ObterSugestoesDespesas(string termoBusca);
    Task<Result<ICollection<LancamentoSugestaoDto>>> ObterSugestoesReceitas(string termoBusca);

    Task<Result<int>> InserirReceita(LancamentoCadastroDto lancamento);
    Task<Result<int>> InserirDespesa(LancamentoCadastroDto lancamento);
    Task<Result<LancamentoCadastroDto>> ObterLancamento(int id);
    Task<Result<int>> AlterarLancamento(LancamentoCadastroDto lancamento);
    Task<Result<int>> ExcluirLancamento(int id);

    Task<Result<int>> InserirTransferenciaEntreContas(TransferenciaInsertDto transferencia);
    Task<Result<int>> InserirPagamentoFatura(TransferenciaInsertDto transferencia);
    Task<Result<TransferenciaUpdateDto>> ObterTransferencia(int id);
    Task<Result<int>> AlterarTransferencia(TransferenciaUpdateDto lancamento);
    Task<Result<int>> ExcluirTransferencia(int idLancamentoOrigem);
}
