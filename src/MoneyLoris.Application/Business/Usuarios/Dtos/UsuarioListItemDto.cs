using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Utils;

namespace MoneyLoris.Application.Business.Usuarios.Dtos;
public class UsuarioListItemDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = default!;
    public DateTime? UltimoLogin { get; set; } = default!;
    public bool Ativo { get; set; } = default!;
    public string Perfil { get; set; } = default!;


    public UsuarioListItemDto()
    {
    }

    public UsuarioListItemDto(Usuario usuario)
    {
        Id = usuario.Id;
        Nome = usuario.Nome;
        UltimoLogin = usuario.UltimoLogin;
        Ativo = usuario.Ativo;
        Perfil = usuario.IdPerfil.ObterDescricao();
    }
}
