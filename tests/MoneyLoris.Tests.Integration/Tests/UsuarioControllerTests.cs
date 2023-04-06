using System.Net;
using System.Net.Http.Json;
using MoneyLoris.Application.Business.Usuarios.Dtos;
using MoneyLoris.Application.Common.Base;
using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Shared;
using MoneyLoris.Application.Utils;
using MoneyLoris.Tests.Integration.Setup.Utils;
using MoneyLoris.Tests.Integration.Tests.Base;

namespace MoneyLoris.Tests.Integration.Tests;
public class UsuarioControllerTests : IntegrationTestsBase
{
    private readonly string SENHA_PADRAO_SHA256 = "2010992D6EA323E39585AC882D3BE4C5457520E651CDD5770EEA40EDADCF02D0"; //dinheiro

    public UsuarioControllerTests()
    {
        //seta relógio do sistema para todos os testes desta classe
        SystemTime.Now = () => new DateTime(2023, 1, 2, 18, 57, 30);
    }

    #region helpers para testes

    private Usuario mockUsuario(PerfilUsuario perfil, bool ativo = true, bool alterarSenha = false)
    {
        var conta = new Usuario
        {
            Nome = "Mariana Rangel Lorencini",
            IdPerfil = perfil,
            Login = "mari.rangel",
            Senha = "123",
            AlterarSenha = alterarSenha,
            DataCriacao = SystemTime.Today().AddDays(-8),
            UltimoLogin = SystemTime.Today().AddDays(-5),
        };

        if (ativo)
        {
            conta.Ativo = true;
            conta.DataInativacao = null;
        }
        else
        {
            conta.Ativo = false;
            conta.DataInativacao = SystemTime.Today().AddDays(-2);
        }

        return conta;
    }


    private async Task<T> salvaEntidadeDb<T>(T entidade) where T : EntityBase
    {
        var dbset = Context.Set<T>();

        var eec = await dbset.AddAsync(entidade);
        await Context.SaveChangesAsync();
        return eec.Entity;
    }

    #endregion

    #region acesso

    [Fact]
    public async Task AcessarTelaUsuarios_UsuarioAdmin_Retorna200()
    {
        //Arrange
        CriarClient(perfil: PerfilUsuario.Administrador);

        //Act
        var response = await HttpClient.GetAsync("/usuario");

        //Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task AcessarTelaUsuarios_UsuarioComum_Retorna403()
    {
        //Arrange
        CriarClient(perfil: PerfilUsuario.Usuario);

        //Act
        var response = await HttpClient.GetAsync("/usuario");

        //Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AcessarTelaUsuarios_UsuarioNaoLogado_Retorna403()
    {
        //Arrange
        CriarClient(logado: false);

        //Act
        var response = await HttpClient.GetAsync("/usuario");

        //Assert
        Assert.Equal(HttpStatusCode.Found, response.StatusCode); //302
    }

    #endregion

    #region inserir

    [Fact]
    public async Task Inserir_DadosCorretosAdministrador_UsuarioCriado()
    {
        //Arrange
        CriarClient(perfil: PerfilUsuario.Administrador);

        //Act
        var dto = new UsuarioCriacaoInputDto
        {
            IdPerfil = PerfilUsuario.Administrador,
            Nome = "Marcos Antônio Lira",
            Login = "marcos.lira"
        };

        var response = await HttpClient.PostAsJsonAsync("/usuario/inserir", dto);

        //Assert
        var idUsuario = await response.AssertResultOk<int>();

        var usuario = await Context.Usuarios.FindAsync(idUsuario);

        Assert.NotNull(usuario);
        Assert.Equal(dto.Nome, usuario!.Nome);
        Assert.Equal(dto.Login, usuario.Login);

        Assert.Equal(SENHA_PADRAO_SHA256, usuario.Senha);
        Assert.True(usuario.AlterarSenha);

        Assert.True(usuario.Ativo);
        Assert.Equal(SystemTime.Now(), usuario.DataCriacao);

        Assert.Null(usuario.UltimoLogin);
        Assert.Null(usuario.DataInativacao);
    }

    [Fact]
    public async Task Inserir_DadosCorretosUsuarioComum_UsuarioCriado()
    {
        //Arrange
        CriarClient(perfil: PerfilUsuario.Administrador);

        //Act
        var dto = new UsuarioCriacaoInputDto
        {
            IdPerfil = PerfilUsuario.Usuario,
            Nome = "Maria Joaquina de Barros",
            Login = "maria.barros"
        };

        var response = await HttpClient.PostAsJsonAsync("/usuario/inserir", dto);

        //Assert
        var idConta = await response.AssertResultOk<int>();

        var conta = await Context.Usuarios.FindAsync(idConta);

        Assert.NotNull(conta);
        Assert.Equal(dto.Nome, conta!.Nome);
        Assert.Equal(dto.Login, conta.Login);

        Assert.Equal(SENHA_PADRAO_SHA256, conta.Senha);
        Assert.True(conta.AlterarSenha);

        Assert.True(conta.Ativo);
        Assert.Equal(SystemTime.Now(), conta.DataCriacao);

        Assert.Null(conta.UltimoLogin);
        Assert.Null(conta.DataInativacao);

        //ideia: no futuro criar conta padrão 'carteira' e categorias base para o usuario, e verificar aqui
    }

    [Fact]
    public async Task Inserir_DadosIncorretos_Erro()
    {
        //Arrange
        CriarClient(perfil: PerfilUsuario.Administrador);

        //Act
        var response = await HttpClient.PostAsJsonAsync("/usuario/inserir",
            new UsuarioCriacaoInputDto
            {
                IdPerfil = PerfilUsuario.Usuario,
                Nome = "",
                Login = "maria.barros"
            });

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.Usuario_CamposObrigatorios);
    }

    #endregion

    #region info

    [Fact]
    public async Task Info_UsuarioExiste_Retorna()
    {
        //Arrange
        CriarClient(perfil: PerfilUsuario.Administrador);

        var contaEf = await salvaEntidadeDb(mockUsuario(PerfilUsuario.Administrador));

        //Act
        var response = await HttpClient.GetAsync($"/usuario/info/{contaEf.Id}");

        //Assert
        var dto = await response.AssertResultOk<UsuarioInfoDto>();

        Assert.NotNull(dto);
        Assert.Equal(dto.Nome, contaEf.Nome);
        Assert.Equal(dto.IdPerfil, (byte)contaEf.IdPerfil);
        Assert.Equal(dto.Perfil, PerfilUsuario.Administrador.ObterDescricao());
        Assert.Equal(dto.Login, contaEf.Login);
        Assert.Equal(dto.UltimoLogin, contaEf.UltimoLogin);
        Assert.Equal(dto.DataInativacao, contaEf.DataInativacao);
    }

    [Fact]
    public async Task Info_UsuarioNaoExiste_RetornaErro()
    {
        //Arrange
        CriarClient(perfil: PerfilUsuario.Administrador);

        //Act
        var response = await HttpClient.GetAsync($"/usuario/info/33");

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.Usuario_NaoEncontrado);
    }

    #endregion

    #region alterar

    [Fact]
    public async Task Alterar_UsuarioExiste_FazAlteracaoNomeLogin()
    {
        //Arrange
        CriarClient(perfil: PerfilUsuario.Administrador);

        var contaDb = await salvaEntidadeDb(mockUsuario(PerfilUsuario.Administrador));

        var contaId = contaDb.Id;

        //Act
        var response = await HttpClient.PostAsJsonAsync("/usuario/alterar",
            new UsuarioAlteracaoDto
            {
                Id = contaId,
                Nome = "Marcela Silva Sauro",
                Login = "marcela.sauro"
            });

        //Assert
        await response.AssertResultOk<int>();

        var cc = mockUsuario(PerfilUsuario.Administrador); //controle

        var contaEf = await Context.Usuarios.FindAsync(contaId);

        Assert.NotNull(contaEf);

        Assert.Equal("Marcela Silva Sauro", contaEf!.Nome);
        Assert.Equal("marcela.sauro", contaEf.Login);

        Assert.Equal(cc.IdPerfil, contaEf.IdPerfil);
        Assert.Equal(cc.Senha, contaEf.Senha);
        Assert.Equal(cc.AlterarSenha, contaEf.AlterarSenha);
        Assert.Equal(cc.DataCriacao, contaEf.DataCriacao);
        Assert.Equal(cc.UltimoLogin, contaEf.UltimoLogin);
        Assert.Equal(cc.Ativo, contaEf.Ativo);
        Assert.Equal(cc.DataInativacao, contaEf.DataInativacao);
    }

    [Fact]
    public async Task Alterar_UsuarioNaoExiste_RetornaErro()
    {
        //Arrange
        CriarClient(perfil: PerfilUsuario.Administrador);

        //Act
        var response = await HttpClient.PostAsJsonAsync("/usuario/alterar",
           new UsuarioAlteracaoDto
           {
               Id = 876,
               Nome = "Marcela Silva Sauro",
               Login = "marcela.sauro"
           });

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.Usuario_NaoEncontrado);
    }

    #endregion

    #region marcar para alterar senha

    [Fact]
    public async Task MarcarAlterarSenha_UsuarioExiste_FazMarcacao()
    {
        //Arrange
        CriarClient(perfil: PerfilUsuario.Administrador);

        var contaDb = await salvaEntidadeDb(mockUsuario(PerfilUsuario.Administrador));

        var contaId = contaDb.Id;


        //Act
        var response = await HttpClient.PostAsJsonAsync("/usuario/redefinirSenha", contaId);

        //Assert
        await response.AssertResultOk<int>();

        var cc = mockUsuario(PerfilUsuario.Administrador); //controle

        var contaEf = await Context.Usuarios.FindAsync(contaId);

        Assert.NotNull(contaEf);
        Assert.Equal(cc.Nome, contaEf!.Nome);
        Assert.Equal(cc.Login, contaEf.Login);
        Assert.Equal(cc.IdPerfil, contaEf.IdPerfil);
        
        Assert.Equal(SENHA_PADRAO_SHA256, contaEf.Senha);
        Assert.True(contaEf.AlterarSenha);

        Assert.Equal(cc.DataCriacao, contaEf.DataCriacao);
        Assert.Equal(cc.UltimoLogin, contaEf.UltimoLogin);
        Assert.Equal(cc.Ativo, contaEf.Ativo);
        Assert.Equal(cc.DataInativacao, contaEf.DataInativacao);
    }

    [Fact]
    public async Task MarcarAlterarSenha_UsuarioJaMarcado_RetornaErro()
    {
        //Arrange
        CriarClient(perfil: PerfilUsuario.Administrador);

        var mc = mockUsuario(PerfilUsuario.Administrador, alterarSenha: true);

        var contaDb = await salvaEntidadeDb(mc);
        var contaId = contaDb.Id;

        //Act
        var response = await HttpClient.PostAsJsonAsync("/usuario/redefinirSenha", contaId);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.Usuario_JaMarcadoParaAlterarSenha);
    }

    [Fact]
    public async Task MarcarAlterarSenha_UsuarioNaoExiste_RetornaErro()
    {
        //Arrange
        CriarClient(perfil: PerfilUsuario.Administrador);

        //Act
        var response = await HttpClient.PostAsJsonAsync("/usuario/redefinirSenha", 1234);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.Usuario_NaoEncontrado);
    }

    #endregion    

    #region inativar

    [Fact]
    public async Task Inativar_UsuarioAtivo_FazInativacao()
    {
        //Arrange
        CriarClient(perfil: PerfilUsuario.Administrador);

        var contaDb = await salvaEntidadeDb(mockUsuario(PerfilUsuario.Administrador));
        var contaId = contaDb.Id;


        //Act
        var response = await HttpClient.PostAsJsonAsync("/usuario/inativar", contaId);

        //Assert
        await response.AssertResultOk<int>();

        var cc = mockUsuario(PerfilUsuario.Administrador); //controle

        var contaEf = await Context.Usuarios.FindAsync(contaId);

        Assert.NotNull(contaEf);
        Assert.Equal(cc.Nome, contaEf!.Nome);
        Assert.Equal(cc.Login, contaEf.Login);
        Assert.Equal(cc.IdPerfil, contaEf.IdPerfil);
        Assert.Equal(cc.Senha, contaEf.Senha);
        Assert.Equal(cc.AlterarSenha, contaEf.AlterarSenha);

        Assert.False(contaEf.Ativo);
        Assert.Equal(SystemTime.Now(), contaEf.DataInativacao);

        Assert.Equal(cc.DataCriacao, contaEf.DataCriacao);
        Assert.Equal(cc.UltimoLogin, contaEf.UltimoLogin);
    }

    [Fact]
    public async Task Inativar_UsuarioJaEstaInativo_RetornaErro()
    {
        //Arrange
        CriarClient(perfil: PerfilUsuario.Administrador);

        var mc = mockUsuario(PerfilUsuario.Administrador, ativo: false);

        var contaDb = await salvaEntidadeDb(mc);
        var contaId = contaDb.Id;

        //Act
        var response = await HttpClient.PostAsJsonAsync("/usuario/inativar", contaId);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.Usuario_JaEstaInativo);
    }

    [Fact]
    public async Task Inativar_UsuarioNaoExiste_RetornaErro()
    {
        //Arrange
        CriarClient(perfil: PerfilUsuario.Administrador);

        //Act
        var response = await HttpClient.PostAsJsonAsync("/usuario/inativar", 1234);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.Usuario_NaoEncontrado);
    }

    #endregion

    #region reativar

    [Fact]
    public async Task Reativar_UsuarioInativo_FazReativacao()
    {
        //Arrange
        CriarClient(perfil: PerfilUsuario.Administrador);

        var mc = mockUsuario(PerfilUsuario.Administrador, ativo: false);

        var contaDb = await salvaEntidadeDb(mc);
        var contaId = contaDb.Id;


        //Act
        var response = await HttpClient.PostAsJsonAsync("/usuario/reativar", contaId);

        //Assert
        await response.AssertResultOk<int>();

        var cc = mockUsuario(PerfilUsuario.Administrador); //controle

        var contaEf = await Context.Usuarios.FindAsync(contaId);

        Assert.NotNull(contaEf);
        Assert.Equal(cc.Nome, contaEf!.Nome);
        Assert.Equal(cc.Login, contaEf.Login);
        Assert.Equal(cc.IdPerfil, contaEf.IdPerfil);
        Assert.Equal(cc.Senha, contaEf.Senha);
        Assert.Equal(cc.AlterarSenha, contaEf.AlterarSenha);

        Assert.True(contaEf.Ativo);
        Assert.Null(contaEf.DataInativacao);

        Assert.Equal(cc.DataCriacao, contaEf.DataCriacao);
        Assert.Equal(cc.UltimoLogin, contaEf.UltimoLogin);
    }

    [Fact]
    public async Task Reativar_UsuarioJaEstaAtivo_RetornaErro()
    {
        //Arrange
        CriarClient(perfil: PerfilUsuario.Administrador);

        var mc = mockUsuario(PerfilUsuario.Administrador);

        var contaDb = await salvaEntidadeDb(mc);
        var contaId = contaDb.Id;

        //Act
        var response = await HttpClient.PostAsJsonAsync("/usuario/reativar", contaId);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.Usuario_JaEstaAtivo);
    }

    [Fact]
    public async Task Reativar_UsuarioNaoExiste_RetornaErro()
    {
        //Arrange
        CriarClient(perfil: PerfilUsuario.Administrador);

        //Act
        var response = await HttpClient.PostAsJsonAsync("/usuario/reativar", 1234);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.Usuario_NaoEncontrado);
    }

    #endregion

    #region excluir

    [Fact]
    public async Task Excluir_Administrador_UsuarioExiste_RealizaExclusao()
    {
        //Arrange
        CriarClient(perfil: PerfilUsuario.Administrador);

        var contaDb = await salvaEntidadeDb(mockUsuario(PerfilUsuario.Administrador));
        var contaId = contaDb.Id;

        //Act
        var response = await HttpClient.PostAsJsonAsync("/usuario/excluir", contaId);

        //Assert
        await response.AssertResultOk<int>();

        var contaEf = await Context.Usuarios.FindAsync(contaId);
        Assert.Null(contaEf);
    }

    [Fact]
    public async Task Excluir_UsuarioComum_UsuarioExiste_RealizaExclusao()
    {
        //Arrange
        CriarClient(perfil: PerfilUsuario.Administrador);

        var contaDb = await salvaEntidadeDb(mockUsuario(PerfilUsuario.Usuario));
        var contaId = contaDb.Id;

        //Act
        var response = await HttpClient.PostAsJsonAsync("/usuario/excluir", contaId);

        //Assert
        await response.AssertResultOk<int>();

        var contaEf = await Context.Usuarios.FindAsync(contaId);
        Assert.Null(contaEf);
    }

    [Fact]
    public async Task Excluir_UsuarioNaoExiste_RetornaErro()
    {
        //Arrange
        CriarClient(perfil: PerfilUsuario.Administrador);

        //Act
        var response = await HttpClient.PostAsJsonAsync("/usuario/excluir", 1234);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.Usuario_NaoEncontrado);
    }

    //[Fact]
    //public async Task Excluir_UsuarioPossuiLancamento_RetornaErro()
    //{
    //}

    #endregion
}
