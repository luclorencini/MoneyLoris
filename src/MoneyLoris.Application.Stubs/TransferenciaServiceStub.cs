using MoneyLoris.Application.Business.Lancamentos.Dtos;
using MoneyLoris.Application.Business.Lancamentos.Interfaces;
using MoneyLoris.Application.Common.Base;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Stubs;
public class TransferenciaServiceStub : ServiceBase, ITransferenciaService
{
    public async Task<Result<int>> InserirTransferenciaEntreContas(TransferenciaInsertDto transferencia)
    {
        return await TaskSuccess((123, "Transferência lançada com sucesso."));
    }

    public async Task<Result<int>> InserirPagamentoFatura(TransferenciaInsertDto transferencia)
    {
        return await TaskSuccess((123, "Pagamento de Fatura lançada com sucesso."));
    }

    public async Task<Result<TransferenciaUpdateDto>> Obter(int id)
    {
        var ret = new TransferenciaUpdateDto
        {
            IdLancamentoOrigem = 502,
            IdLancamentoDestino = 503,
            Data = SystemTime.Today().AddDays(-1),
            Tipo = TipoTransferencia.TransferenciaEntreContas,
            IdMeioPagamentoOrigem = 304,  //caixa
            IdMeioPagamentoDestino = 303,  //picpay
            Valor = 300.00M  //Importante: tem que voltar sempre positivo
        };
        return await TaskSuccess(ret);
    }

    public async Task<Result<int>> Alterar(TransferenciaUpdateDto lancamento)
    {
        return await TaskSuccess((123, "Transferência alterada com sucesso."));
    }

    public async Task<Result<int>> Excluir(int idLancamentoOrigem)
    {
        return await TaskSuccess((123, "Transferência excluída com sucesso."));
    }
}
