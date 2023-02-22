using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Utils;

namespace MoneyLoris.Application.Business.Usuarios.Dtos;
public class UsuarioInfoDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = null!;
    public byte IdPerfil { get; set; }
    public string Perfil { get; set; } = null!;
    public string Login { get; set; } = null!;
    public DateTime? UltimoLogin { get; set; }
    public DateTime? DataInativacao { get; set; }

    public UsuarioInfoDto()
    {
    }

    public UsuarioInfoDto(Usuario usuario)
    {
        Id = usuario.Id;
        Nome = usuario.Nome;
        IdPerfil = (byte)usuario.IdPerfil;
        Perfil = usuario.IdPerfil.ObterDescricao();
        Login = usuario.Login;
        UltimoLogin = usuario.UltimoLogin;
        DataInativacao = usuario.DataInativacao;
    }
}
