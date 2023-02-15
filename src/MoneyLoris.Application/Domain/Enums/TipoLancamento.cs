using System.ComponentModel;

namespace MoneyLoris.Application.Domain.Enums;
public enum TipoLancamento : byte
{
    [Description("Receita")]
    Receita = 1,

    [Description("Despesa")]
    Despesa = 2,
}