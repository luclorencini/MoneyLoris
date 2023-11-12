using System.Net.Http.Json;
using MoneyLoris.Application.Business.MeiosPagamento.Dtos;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Shared;
using MoneyLoris.Tests.Integration.Setup.Utils;
using MoneyLoris.Tests.Integration.Tests.Base;

namespace MoneyLoris.Tests.Integration.Tests.MeiosPagamento;
public class ContaController_InserirTests : IntegrationTestsBase
{
    [Fact]
    public async Task Inserir_UsuarioAdmin_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Administrador);
        await DbSeeder.InserirUsuarios();

        //Act
        var dto = new MeioPagamentoCriacaoDto
        {
            Nome = "Carteira",
            Tipo = TipoMeioPagamento.Carteira,
            Cor = "000000",
            Ordem = 1,
        };

        var response = await HttpClient.PostAsJsonAsync("/conta/inserir", dto);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.MeioPagamento_AdminNaoPode);
    }

    [Fact]
    public async Task Inserir_CartaoCredito_SemLimite_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        //Act
        var dto = new MeioPagamentoCriacaoDto
        {
            Nome = "TestCard",
            Tipo = TipoMeioPagamento.CartaoCredito,
            Cor = "000000",
            Ordem = null,

            Limite = null,
            DiaFechamento = 1,
            DiaVencimento = 10
        };

        var response = await HttpClient.PostAsJsonAsync("/conta/inserir", dto);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.MeioPagamento_CamposObrigatorios);
    }

    [Fact]
    public async Task Inserir_CartaoCredito_SemDiaFechamento_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        //Act
        var dto = new MeioPagamentoCriacaoDto
        {
            Nome = "TestCard",
            Tipo = TipoMeioPagamento.CartaoCredito,
            Cor = "000000",
            Ordem = 1,

            Limite = 8000,
            DiaFechamento = null,
            DiaVencimento = 10
        };

        var response = await HttpClient.PostAsJsonAsync("/conta/inserir", dto);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.MeioPagamento_CamposObrigatorios);
    }

    [Fact]
    public async Task Inserir_CartaoCredito_SemDiaVencimento_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        //Act
        var dto = new MeioPagamentoCriacaoDto
        {
            Nome = "TestCard",
            Tipo = TipoMeioPagamento.CartaoCredito,
            Cor = "000000",
            Ordem = 1,

            Limite = 8000,
            DiaFechamento = 2,
            DiaVencimento = null
        };

        var response = await HttpClient.PostAsJsonAsync("/conta/inserir", dto);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.MeioPagamento_CamposObrigatorios);
    }

    [Fact]
    public async Task Inserir_DadosCorretos_NaoEhCartaoCredito_MeioPagamentoCriado()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        //Act
        var dto = new MeioPagamentoCriacaoDto
        {
            Nome = "Carteira",
            Tipo = TipoMeioPagamento.Carteira,
            Cor = "000000",
            Ordem = 1,
        };

        var response = await HttpClient.PostAsJsonAsync("/conta/inserir", dto);

        //Assert
        var idMeio = await response.ConverteResultOk<int>();

        var meio = await Context.MeiosPagamento.FindAsync(idMeio);

        Assert.NotNull(meio);
        Assert.Equal(TestConstants.USUARIO_COMUM_ID, meio!.IdUsuario);
        Assert.Equal(dto.Nome, meio.Nome);
        Assert.Equal(dto.Ordem, meio.Ordem);
        Assert.Equal(dto.Tipo, meio.Tipo);
        Assert.Equal(dto.Cor, meio.Cor);
        Assert.True(meio.Ativo);
        Assert.Null(meio.Limite);
        Assert.Null(meio.DiaFechamento);
        Assert.Null(meio.DiaVencimento);
    }

    [Fact]
    public async Task Inserir_DadosCorretosComCamposExtrasCartao_NaoEhCartaoCredito_MeioPagamentoCriado_IgnoraCamposExtras()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        //Act
        var dto = new MeioPagamentoCriacaoDto
        {
            Nome = "BankTest",
            Tipo = TipoMeioPagamento.ContaCorrente,
            Cor = "000000",
            Ordem = 1,

            Limite = 10000,
            DiaFechamento = 1,
            DiaVencimento = 10
        };

        var response = await HttpClient.PostAsJsonAsync("/conta/inserir", dto);

        //Assert
        var idMeio = await response.ConverteResultOk<int>();

        var meio = await Context.MeiosPagamento.FindAsync(idMeio);

        Assert.NotNull(meio);
        Assert.Equal(TestConstants.USUARIO_COMUM_ID, meio!.IdUsuario);
        Assert.Equal(dto.Nome, meio!.Nome);
        Assert.Equal(dto.Ordem, meio.Ordem);
        Assert.Equal(dto.Tipo, meio.Tipo);
        Assert.Equal(dto.Cor, meio.Cor);
        Assert.True(meio.Ativo);
        Assert.Null(meio.Limite);
        Assert.Null(meio.DiaFechamento);
        Assert.Null(meio.DiaVencimento);
    }

    [Fact]
    public async Task Inserir_DadosCorretos_EhCartaoCredito_MeioPagamentoCriado()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        //Act
        var dto = new MeioPagamentoCriacaoDto
        {
            Nome = "TestCard",
            Tipo = TipoMeioPagamento.CartaoCredito,
            Cor = "000000",
            Ordem = 1,

            Limite = 10000,
            DiaFechamento = 1,
            DiaVencimento = 10
        };

        var response = await HttpClient.PostAsJsonAsync("/conta/inserir", dto);

        //Assert
        var idMeio = await response.ConverteResultOk<int>();

        var meio = await Context.MeiosPagamento.FindAsync(idMeio);

        Assert.NotNull(meio);
        Assert.Equal(TestConstants.USUARIO_COMUM_ID, meio!.IdUsuario);
        Assert.Equal(dto.Nome, meio.Nome);
        Assert.Equal(dto.Ordem, meio.Ordem);
        Assert.Equal(dto.Tipo, meio.Tipo);
        Assert.Equal(dto.Cor, meio.Cor);
        Assert.True(meio.Ativo);
        Assert.Equal(dto.Limite, meio.Limite);
        Assert.Equal(dto.DiaFechamento, meio.DiaFechamento);
        Assert.Equal(dto.DiaVencimento, meio.DiaVencimento);
    }
}
