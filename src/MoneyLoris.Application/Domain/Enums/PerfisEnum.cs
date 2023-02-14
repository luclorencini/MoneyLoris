using System.ComponentModel;

namespace MoneyLoris.Application.Domain.Enums;
public enum PerfisEnum : byte
{
    [Description("Administrador")]
    Administrador = 1,

    [Description("Usuário")]
    Usuario = 2
}
