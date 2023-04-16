using MoneyLoris.Application.Business.Lancamentos.Dtos;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Business.Lancamentos.Interfaces;
public interface ILancamentoService
{
    Task<Result<Pagination<ICollection<LancamentoListItemDto>>>> Pesquisar(LancamentoFiltroDto filtro);
    Task<Result<LancamentoBalancoDto>> ObterBalanco(LancamentoFiltroDto filtro);

    Task<Result<ICollection<LancamentoSugestaoDto>>> ObterSugestoesDespesas(string termoBusca);
    Task<Result<ICollection<LancamentoSugestaoDto>>> ObterSugestoesReceitas(string termoBusca);

    Task<Result<int>> InserirReceita(LancamentoCadastroDto dto);
    Task<Result<int>> InserirDespesa(LancamentoCadastroDto dto);
    Task<Result<LancamentoCadastroDto>> Obter(int id);
    Task<Result<int>> Alterar(LancamentoCadastroDto dto);
    Task<Result<int>> Excluir(int id);
}
