using System.ComponentModel;

namespace MoneyLoris.Application.Domain.Enums;
public enum RegimeContabil : byte
{
    [Description("Regime de Caixa")]
    Caixa = 1,

    [Description("Regime de Competência")]
    Competencia = 2
}
