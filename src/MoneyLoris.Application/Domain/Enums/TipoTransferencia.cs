using System.ComponentModel;

namespace MoneyLoris.Application.Domain.Enums;
public enum TipoTransferencia : byte
{
    [Description("Transferência entre Contas")]
    TransferenciaEntreContas = 1,

    [Description("Pagamento de Fatura")]
    PagamentoFatura = 2,
}
