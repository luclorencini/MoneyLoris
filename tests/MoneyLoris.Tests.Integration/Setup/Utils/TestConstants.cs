using Microsoft.AspNetCore.Http;
using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Tests.Integration.Setup.Utils;
public static class TestConstants
{
    public static string SENHA_SHA256_123456 = "8D969EEF6ECAD3C29A3A629280E686CF0C3F5D5A86AFF3CA12020C923ADC6C92"; //123456
    public static string SENHA_SHA256_777888 = "6CCF5EB0B98684778C3B1A5415FDEECD6819DD2EF1CFB22EEE2C775CC41DC9CF"; //777888

    public const int USUARIO_ADMIN_ID = 1;
    public const int USUARIO_COMUM_ID = 2;
    public const int USUARIO_COMUM_B_ID = 3;

    public static Usuario UsuarioAdmin()
    {
        return new Usuario
        {
            Id = USUARIO_ADMIN_ID,
            IdPerfil = PerfilUsuario.Administrador,
            Nome = "Administrador",
            Login = "admin",
            Senha = TestConstants.SENHA_SHA256_123456,
            Ativo = true,
            AlterarSenha = false,
            DataCriacao = SystemTime.Now()
        };
    }

    public static Usuario UsuarioComum()
    {
        return new Usuario
        {
            Id = USUARIO_COMUM_ID,
            IdPerfil = PerfilUsuario.Usuario,
            Nome = "Usuário",
            Login = "usuario",
            Senha = TestConstants.SENHA_SHA256_123456,
            Ativo = true,
            AlterarSenha = false,
            DataCriacao = SystemTime.Now()
        };
    }

    public static Usuario UsuarioComumB()
    {
        return new Usuario
        {
            Id = USUARIO_COMUM_B_ID,
            IdPerfil = PerfilUsuario.Usuario,
            Nome = "Usuário B",
            Login = "usuariob",
            Senha = TestConstants.SENHA_SHA256_123456,
            Ativo = true,
            AlterarSenha = false,
            DataCriacao = SystemTime.Now()
        };
    }
}
