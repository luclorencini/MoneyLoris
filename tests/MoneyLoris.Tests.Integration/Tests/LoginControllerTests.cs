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

    private async Task ArrangeUsuario(bool ativo = true, bool alterarSenha = false)
    {
        await Context.Usuarios.AddAsync(new Usuario
        {
            Nome = "Admin",
            IdPerfil = PerfilUsuario.Administrador,
            Login = "admin",
            Senha = "A665A45920422F9D417E4867EFDC4FB8A04A1F3FFF1FA07E998E86F7F7A27AE3", //senha: 123
            DataCriacao = SystemTime.Now().AddDays(-1),
            Ativo = ativo,
            AlterarSenha = alterarSenha
        });

        await Context.SaveChangesAsync();
    }

    [Fact]
    public async Task SignIn_Sucesso_UsuarioAdmin_RetornaUrlTelaUsuario()
    {
        //Arrange
        CriarClient(logado: false);

        await ArrangeUsuario();

        //Act
        var response = await HttpClient.PostAsJsonAsync("/Login/SignIn",
            new LoginInputDto
            {
                Login = "admin",
                Senha = "123",
                ManterConectado = true
            });

        //Assert
        var retorno = await response.AssertResultOk<LoginRetornoDto>();

        Assert.Equal("usuario", retorno.UrlRedir); // admin: redireciona pra tela de usuarios
        Assert.Null(retorno.AlterarSenha);

        var usuario = Context.Usuarios.FirstOrDefault(c => c.Login == "admin");
        Assert.NotNull(usuario);
        Assert.Equal(usuario!.UltimoLogin, SystemTime.Now());
    }

    [Fact]
    public async Task SignIn_NovoUsuarioMarcadoPraAlterarSenha_RetornaAlterarSenhaTrue()
    {
        //Arrange
        CriarClient(logado: false);

        await ArrangeUsuario(alterarSenha: true);

        //Act
        var response = await HttpClient.PostAsJsonAsync("/Login/SignIn",
            new LoginInputDto
            {
                Login = "admin",
                Senha = "123",
                ManterConectado = true
            });

        //Assert
        var retorno = await response.AssertResultOk<LoginRetornoDto>();

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
        CriarClient(logado: false);

        await ArrangeUsuario();

        //Act
        var response = await HttpClient.PostAsJsonAsync("/Login/SignIn",
            new LoginInputDto
            {
                Login = "admino",
                Senha = "123",
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
        CriarClient(logado: false);

        await ArrangeUsuario();

        //Act
        var response = await HttpClient.PostAsJsonAsync("/Login/SignIn",
            new LoginInputDto
            {
                Login = "admin",
                Senha = "1234",
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
        CriarClient(logado: false);

        await ArrangeUsuario(ativo: false);

        //Act
        var response = await HttpClient.PostAsJsonAsync("/Login/SignIn",
            new LoginInputDto
            {
                Login = "admin",
                Senha = "123",
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
        CriarClient(logado: false);

        await ArrangeUsuario();

        //Act
        var response = await HttpClient.PostAsJsonAsync("/Login/AlterarSenha",
            new AlteracaoSenhaDto
            {
                Login = "admin",
                SenhaAtual = "123",
                NovaSenha = "123456"
            });

        //Assert
        response.EnsureSuccessStatusCode();
    }


    [Fact]
    public async Task TelaInicial_Logado_Retorna200()
    {
        //Arrange
        CriarClient(perfil: PerfilUsuario.Administrador);

        //Act
        var response = await HttpClient.GetAsync("/lancamento");

        //Assert
        response.EnsureSuccessStatusCode();
    }
}
