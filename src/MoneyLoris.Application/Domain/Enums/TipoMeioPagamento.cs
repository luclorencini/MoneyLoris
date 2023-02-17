using System.ComponentModel;

namespace MoneyLoris.Application.Domain.Enums;
public enum TipoMeioPagamento : byte
{
    [Description("Carteira")]
    Carteira = 1,

    [Description("Conta Corrente")]
    ContaCorrente = 2,

    [Description("Cartão de Crédito")]
    CartaoCredito = 3,

    [Description("Poupança")]
    Poupanca = 4,

    [Description("Conta de Pagamento")]
    ContaPagamento = 5,

    [Description("Carteira Digital")]
    CarteiraDigital = 6,

    [Description("Cartão de Benefício")]
    CartaoBeneficio = 7,

    [Description("Outros")]
    Outros = 99
}
