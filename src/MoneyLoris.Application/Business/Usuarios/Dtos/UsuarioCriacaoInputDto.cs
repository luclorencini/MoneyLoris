using MoneyLoris.Application.Domain.Enums;

namespace MoneyLoris.Application.Business.Usuarios.Dtos;
public class UsuarioCriacaoInputDto
{
    public PerfilUsuario IdPerfil { get; set; }
    public string Nome { get; set; } = null!;
    public string Login { get; set; } = null!;
}
