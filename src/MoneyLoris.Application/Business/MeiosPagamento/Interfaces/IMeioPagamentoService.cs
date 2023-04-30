using MoneyLoris.Application.Business.MeiosPagamento.Dtos;
using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Business.MeiosPagamento.Interfaces;
public interface IMeioPagamentoService
{
    Task<Result<ICollection<MeioPagamentoCadastroListItemDto>>> Listar();

    Task<Result<MeioPagamentoCadastroDto>> Obter(int id);
    Task<Result<int>> Inserir(MeioPagamentoCriacaoDto dto);
    Task<Result<int>> Alterar(MeioPagamentoCadastroDto dto);
    Task<Result<int>> Excluir(int id);

    Task<Result> Inativar(int id);
    Task<Result> Reativar(int id);

    Task<decimal?> RecalcularSaldo(MeioPagamento meio, decimal valorDelta);

    Task<Result<ICollection<MeioPagamentoListItemDto>>> ObterMeiosPagamento();
    Task<Result<ICollection<MeioPagamentoListItemDto>>> ObterContas();
    Task<Result<ICollection<MeioPagamentoListItemDto>>> ObterCartoes();
}
