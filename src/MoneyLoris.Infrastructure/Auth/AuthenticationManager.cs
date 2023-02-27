using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MoneyLoris.Application.Business.Auth.Interfaces;
using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Infrastructure.Auth;
public class AuthenticationManager : IAuthenticationManager
{
    private readonly IHttpContextAccessor _httpContextAccessor = null!;
    //private readonly AuthConfig _authConfig = null!;

    public AuthenticationManager()
    {

    }

    public AuthenticationManager(IHttpContextAccessor httpContextAccessor, IOptions<AuthConfig> authConfigOptions)
    {
        _httpContextAccessor = httpContextAccessor;
        //_authConfig = authConfigOptions.Value;
    }

    public async Task Login(Usuario usuario, bool isPersistent)
    {
        //cria o usuario com suas Claims
        var claims = GerarClaimsUsuario(usuario);
        //var identity = new ClaimsIdentity(claims, _authConfig.Scheme);
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var userPrincipal = new ClaimsPrincipal(identity);

        //manter conectado: cookie persistente, login dura 2 semanas; senão, login padrão de 1 hora de duração
        //var authProperties = new AuthenticationProperties
        //{
        //    AllowRefresh = true,
        //    IsPersistent = isPersistent,
        //    ExpiresUtc = isPersistent ? DateTime.Now.AddDays(14) : DateTime.Now.AddMinutes(60)
        //};

        var authProperties = new AuthenticationProperties
        {
            AllowRefresh = true,
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(14),
            IssuedUtc = DateTimeOffset.UtcNow
        };

        //await _httpContextAccessor.HttpContext.SignInAsync(_authConfig.Scheme, userPrincipal, authProperties);

        await _httpContextAccessor.HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            userPrincipal,
            authProperties);
    }

    public async Task LogOut()
    {
        if (_httpContextAccessor.HttpContext.User.Identity!.IsAuthenticated)
        {
            //await _httpContextAccessor.HttpContext.SignOutAsync(_authConfig.Scheme);
            await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
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

    public UserAuthInfo ObterInfoUsuarioLogado()
    {
        var claims = _httpContextAccessor.HttpContext.User.Claims;

        var id = claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
        var nome = claims.Where(c => c.Type == ClaimTypes.Name).Select(c => c.Value).SingleOrDefault();
        var role = claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).SingleOrDefault();

        var info = new UserAuthInfo
        {
            Id = Convert.ToInt32(id),
            UserName = nome!,
            IsAdmin = role == "Administrador"
        };

        return info;
    }
}
