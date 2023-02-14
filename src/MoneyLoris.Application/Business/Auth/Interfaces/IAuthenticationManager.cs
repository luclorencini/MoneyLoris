using System.Security.Claims;
using MoneyLoris.Application.Domain.Entities;

namespace MoneyLoris.Application.Business.Auth.Interfaces;
public interface IAuthenticationManager
{
    Task Login(Usuario usuario, bool isPersistent);
    Task LogOut();

    ICollection<Claim> GerarClaimsUsuario(Usuario usuario);
}
