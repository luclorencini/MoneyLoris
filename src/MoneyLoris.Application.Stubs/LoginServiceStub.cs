using MoneyLoris.Application.Business.Auth.Dtos;
using MoneyLoris.Application.Business.Auth.Interfaces;
using MoneyLoris.Application.Common.Base;
using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Stubs;

public class LoginServiceStub : ServiceBase, ILoginService
{
    private readonly IAuthenticationManager _authManager;

    public LoginServiceStub(IAuthenticationManager authManager)
    {
        _authManager = authManager;
    }

    public async Task<Result<bool>> Login(LoginInputDto login)
    {
        await realizarLogin(login.Login);

        return false; // Alterar senha: false -> te manda direto pra tela inicial
        //return true; //Alterar senha: true -> te mostra a tela de alterar senha
    }

    public async Task<Result> AlterarSenha(AlteracaoSenhaDto dto)
    {
        await realizarLogin(dto.Login);

        return Result.Success();
    }

    private async Task realizarLogin(string login)
    {
        Usuario usuario = null!;

        if (login == "admin")
        {
            usuario = new Usuario { Nome = "Admin", Id = 1234, IdPerfil = PerfilUsuario.Administrador };
        }
        else
        {
            usuario = new Usuario { Nome = "Ronaldo", Id = 1234, IdPerfil = PerfilUsuario.Usuario };
        }
        await _authManager.Login(usuario, isPersistent: false);
    }


    public async Task<Result> LogOut()
    {
        await _authManager.LogOut();

        return Result.Success();
    }
}
