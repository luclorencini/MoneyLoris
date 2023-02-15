using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MoneyLoris.Application.Business.Auth.Interfaces;
using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Domain.Enums;

namespace MoneyLoris.Infrastructure.Auth;
public class AuthenticationManager : IAuthenticationManager
{
    private readonly IHttpContextAccessor _httpContextAccessor = null!;
    private readonly AuthConfig _authConfig = null!;

    public AuthenticationManager()
    {

    }

    public AuthenticationManager(IHttpContextAccessor httpContextAccessor, IOptions<AuthConfig> authConfigOptions)
    {
        _httpContextAccessor = httpContextAccessor;
        _authConfig = authConfigOptions.Value;
    }

    public async Task Login(Usuario usuario, bool isPersistent)
    {
        //cria o usuario com suas Claims
        var claims = GerarClaimsUsuario(usuario);
        var identity = new ClaimsIdentity(claims, _authConfig.Scheme);
        var userPrincipal = new ClaimsPrincipal(identity);

        //manter conectado: cookie persistente, login dura 2 semanas; senão, login padrão de 1 hora de duração
        await _httpContextAccessor.HttpContext.SignInAsync(userPrincipal,
            new AuthenticationProperties
            {
                IsPersistent = isPersistent,
                ExpiresUtc = isPersistent ? DateTime.Now.AddDays(14) : DateTime.Now.AddMinutes(60)
            });
    }

    public async Task LogOut()
    {
        if (_httpContextAccessor.HttpContext.User.Identity!.IsAuthenticated)
        {
            await _httpContextAccessor.HttpContext.SignOutAsync();
        }
    }

    public ICollection<Claim> GerarClaimsUsuario(Usuario usuario)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.Nome)
        };

        if (usuario.IdPerfil == PerfilUsuario.Administrador)
        {
            claims.Add(new Claim(ClaimTypes.Role, "Administrador"));
        }
        else if (usuario.IdPerfil == PerfilUsuario.Usuario)
        {
            claims.Add(new Claim(ClaimTypes.Role, "Usuario"));
        }

        return claims;
    }
}
