using System.Security.Claims;
using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Business.Auth.Interfaces;
public interface IAuthenticationManager
{
    Task Login(Usuario usuario, bool isPersistent);
    Task LogOut();

    ICollection<Claim> GerarClaimsUsuario(Usuario usuario);

    UserAuthInfo ObterInfoUsuarioLogado();
}
