using MoneyLoris.Application.Business.Lancamentos.Dtos;
using MoneyLoris.Application.Business.Lancamentos.Interfaces;
using MoneyLoris.Application.Common.Base;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Stubs;
public class LancamentoServiceStub : ServiceBase, ILancamentoService
{
    public async Task<Result<int>> InserirReceita(LancamentoCadastroDto lancamento)
    {
        return await TaskSuccess((123, "Receita lançada com sucesso."));
    }

    public async Task<Result<int>> InserirDespesa(LancamentoCadastroDto lancamento)
    {
        return await TaskSuccess((123, "Despesa lançada com sucesso."));
    }

    public async Task<Result<LancamentoInfoDto>> Obter(int id)
    {
        var ret = new LancamentoInfoDto
        {
            Id = 502,
            Data = SystemTime.Today().AddDays(-1),
            Tipo = TipoLancamento.Despesa,
            IdMeioPagamento = 302,  //nubank
            IdCategoria = 101,  //Alimentação
            IdSubcategoria = 213,  //Lanche
            Descricao = "Mc Donald's",
            Valor = 49.50M  //Importante: tem que voltar sempre positivo
        };
        return await TaskSuccess(ret);
    }

    public async Task<Result<int>> Alterar(LancamentoEdicaoDto lancamento)
    {
        return await TaskSuccess((123, "Lançamento alterado com sucesso."));
    }

    public async Task<Result<int>> Excluir(int id)
    {
        return await TaskSuccess((123, "Lançamento excluído com sucesso."));
    }
}
