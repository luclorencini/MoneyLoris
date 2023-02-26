using System.ComponentModel;

namespace MoneyLoris.Application.Domain.Enums;
public enum OperacaoLancamento : byte
{
    [Description("Lançamento Simples")]
    LancamentoSimples = 1,

    [Description("Transferência")]
    Transferencia = 2,
}
