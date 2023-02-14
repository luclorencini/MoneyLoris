using MoneyLoris.Application.Common.Base;
using static MoneyLoris.Application.Domain.Enums;

namespace MoneyLoris.Application.Domain.Entities;
public partial class Usuario : EntityBase
{

    public string Nome { get; set; } = null!;
    public string Login { get; set; } = null!;
    public string Senha { get; set; } = null!;
    public bool Ativo { get; set; }
    public PerfilUsuario IdPerfil { get; set; }

    public bool AlterarSenha { get; set; }

    public DateTime DataCriacao { get; set; }
    public DateTime? UltimoLogin { get; set; }
    public DateTime? DataInativacao { get; set; }
}
