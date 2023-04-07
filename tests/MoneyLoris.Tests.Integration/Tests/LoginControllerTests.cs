using System.Net.Http.Json;
using MoneyLoris.Application.Business.Auth.Dtos;
using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Shared;
using MoneyLoris.Tests.Integration.Setup.Utils;
using MoneyLoris.Tests.Integration.Tests.Base;

namespace MoneyLoris.Tests.Integration.Tests;
public class LoginControllerTests : IntegrationTestsBase
{
    public LoginControllerTests() : base()
    {
        //seta relógio do sistema para todos os testes desta classe
        SystemTime.Now = () => new DateTime(2023, 1, 6, 22, 21, 30);
    }

    private async Task ArrangeUsuario(bool admin = true, bool ativo = true, bool alterarSenha = false)
    {
        var usuario = new Usuario();

        if (admin)
        {
            usuario.Nome = "Admin";
            usuario.IdPerfil = PerfilUsuario.Administrador;
            usuario.Login = "admin";
        }
        else
        {
            usuario.Nome = "Usuario";
            usuario.IdPerfil = PerfilUsuario.Usuario;
            usuario.Login = "usuario";
        }

        usuario.Senha = TestConstants.SENHA_SHA256_123456;
        usuario.DataCriacao = SystemTime.Now().AddDays(-1);
        usuario.Ativo = ativo;
        usuario.AlterarSenha = alterarSenha;

        await Context.Usuarios.AddAsync(usuario);

        await Context.SaveChangesAsync();
    }

    [Fact]
    public async Task SignIn_Sucesso_UsuarioAdmin_RetornaUrlTelaUsuario()
    {
        //Arrange
        SubirAplicacao(logado: false);

        await ArrangeUsuario();

        //Act
        var response = await HttpClient.PostAsJsonAsync("/Login/SignIn",
            new LoginInputDto
            {
                Login = "admin",
                Senha = "123456",
                ManterConectado = true
            });

        //Assert
        var retorno = await response.ConverteResultOk<LoginRetornoDto>();

        Assert.Equal("usuario", retorno.UrlRedir); // admin: redireciona pra tela de usuarios
        Assert.Null(retorno.AlterarSenha);

        var usuario = Context.Usuarios.FirstOrDefault(c => c.Login == "admin");
        Assert.NotNull(usuario);
        Assert.Equal(usuario!.UltimoLogin, SystemTime.Now());
    }

    [Fact]
    public async Task SignIn_Sucesso_UsuarioComum_RetornaUrlTelaLancamento()
    {
        //Arrange
        SubirAplicacao(logado: false);

        await ArrangeUsuario(admin: false);

        //Act
        var response = await HttpClient.PostAsJsonAsync("/Login/SignIn",
            new LoginInputDto
            {
                Login = "usuario",
                Senha = "123456",
                ManterConectado = true
            });

        //Assert
        var retorno = await response.ConverteResultOk<LoginRetornoDto>();

        Assert.Equal("lancamento", retorno.UrlRedir); // admin: redireciona pra tela de lançamento
        Assert.Null(retorno.AlterarSenha);

        var usuario = Context.Usuarios.FirstOrDefault(c => c.Login == "usuario");
        Assert.NotNull(usuario);
        Assert.Equal(usuario!.UltimoLogin, SystemTime.Now());
    }

    [Fact]
    public async Task SignIn_NovoUsuarioMarcadoPraAlterarSenha_RetornaAlterarSenhaTrue()
    {
        //Arrange
        SubirAplicacao(logado: false);

        await ArrangeUsuario(alterarSenha: true);

        //Act
        var response = await HttpClient.PostAsJsonAsync("/Login/SignIn",
            new LoginInputDto
            {
                Login = "admin",
                Senha = "123456",
                ManterConectado = true
            });

        //Assert
        var retorno = await response.ConverteResultOk<LoginRetornoDto>();

        Assert.Null(retorno.UrlRedir); // não liberou login: urlRedir nula
        Assert.True(retorno.AlterarSenha);

        var usuario = Context.Usuarios.FirstOrDefault(c => c.Login == "admin");
        Assert.NotNull(usuario);
        Assert.Null(usuario!.UltimoLogin);
    }

    [Fact]
    public async Task SignIn_UsuarioNaoExiste_Erro()
    {
        //Arrange
        SubirAplicacao(logado: false);

        await ArrangeUsuario();

        //Act
        var response = await HttpClient.PostAsJsonAsync("/Login/SignIn",
            new LoginInputDto
            {
                Login = "ronaldo",
                Senha = "123456",
                ManterConectado = true
            });

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.Login_UsuarioOuSenhaNaoConferem);
    }

    [Fact]
    public async Task SignIn_SenhaErrada_Erro()
    {
        //Arrange
        SubirAplicacao(logado: false);

        await ArrangeUsuario();

        //Act
        var response = await HttpClient.PostAsJsonAsync("/Login/SignIn",
            new LoginInputDto
            {
                Login = "admin",
                Senha = "senhaerrada",
                ManterConectado = true
            });

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.Login_UsuarioOuSenhaNaoConferem);
    }

    [Fact]
    public async Task SignIn_UsuarioInativo_Erro()
    {
        //Arrange
        SubirAplicacao(logado: false);

        await ArrangeUsuario(admin: false, ativo: false);

        //Act
        var response = await HttpClient.PostAsJsonAsync("/Login/SignIn",
            new LoginInputDto
            {
                Login = "usuario",
                Senha = "123456",
                ManterConectado = true
            });

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.Login_UsuarioInativo);
    }

    [Fact]
    public async Task AlterarSenha_NovaSenhaValida_RetornaAlterarSenhaFalse()
    {
        //Arrange
        SubirAplicacao(logado: false);

        await ArrangeUsuario(admin: false);

        //Act
        var response = await HttpClient.PostAsJsonAsync("/Login/AlterarSenha",
            new AlteracaoSenhaDto
            {
                Login = "usuario",
                SenhaAtual = "123456",
                NovaSenha = "777888"
            });

        //Assert
        response.EnsureSuccessStatusCode();

        var usuario = Context.Usuarios.FirstOrDefault(c => c.Login == "usuario");
        Assert.NotNull(usuario);
        Assert.Equal(TestConstants.SENHA_SHA256_777888, usuario!.Senha);
    }

    [Fact]
    public async Task AlterarSenha_NovaSenhaIgualAnterior_RetornaErro()
    {
        //Arrange
        SubirAplicacao(logado: false);

        await ArrangeUsuario(admin: false);

        //Act
        var response = await HttpClient.PostAsJsonAsync("/Login/AlterarSenha",
            new AlteracaoSenhaDto
            {
                Login = "usuario",
                SenhaAtual = "123456",
                NovaSenha = "123456"
            });

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.Usuario_SenhaIgualAnterior);
    }

    [Fact]
    public async Task AlterarSenha_NovaSenhaMuitoCurta_RetornaErro()
    {
        //Arrange
        SubirAplicacao(logado: false);

        await ArrangeUsuario(admin: false);

        //Act
        var response = await HttpClient.PostAsJsonAsync("/Login/AlterarSenha",
            new AlteracaoSenhaDto
            {
                Login = "usuario",
                SenhaAtual = "123456",
                NovaSenha = "12345"
            });

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.Usuario_SenhaInvalida);
    }


    [Fact]
    public async Task TelaInicial_Logado_Retorna200()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Administrador);

        //Act
        var response = await HttpClient.GetAsync("/lancamento");

        //Assert
        response.EnsureSuccessStatusCode();
    }
}
