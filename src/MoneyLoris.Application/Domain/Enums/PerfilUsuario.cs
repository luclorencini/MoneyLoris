using System.ComponentModel;

namespace MoneyLoris.Application.Domain.Enums;
public enum PerfilUsuario : byte
{
    [Description("Administrador")]
    Administrador = 1,

    [Description("Usuário")]
    Usuario = 2
}
