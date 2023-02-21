using System.ComponentModel;

namespace MoneyLoris.Application.Domain.Enums;
public enum TipoTransferencia : byte
{
    [Description("Transfêrencia Entre Contas")]
    TransferenciaEntreContas = 1,

    [Description("Pagamento de Fatura")]
    PagamentoFatura = 2,
}
