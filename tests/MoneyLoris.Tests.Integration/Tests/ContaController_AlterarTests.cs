using System.Net.Http.Json;
using MoneyLoris.Application.Business.MeiosPagamento.Dtos;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Shared;
using MoneyLoris.Tests.Integration.Setup.Utils;
using MoneyLoris.Tests.Integration.Tests.Base;

namespace MoneyLoris.Tests.Integration.Tests;
public class ContaController_AlterarTests : IntegrationTestsBase
{
    [Fact]
    public async Task Alterar_UsuarioAdmin_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Administrador);
        await DbSeeder.InserirUsuarios();

        var mp = await DbSeeder.InserirMeioPagamento();

        //Act
        var dto = new MeioPagamentoCadastroDto
        {
            Id = mp.Id,
            Nome = "Carteira Grande",
            Tipo = TipoMeioPagamento.Carteira,
            Cor = "111111",
            Ordem = 2,
        };

        var response = await HttpClient.PostAsJsonAsync("/conta/alterar", dto);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.MeioPagamento_AdminNaoPode);
    }

    [Fact]
    public async Task Alterar_MeioPagamentoNaoPertenceUsuario_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var mp = await DbSeeder.InserirMeioPagamento(idUsuario: TestConstants.USUARIO_COMUM_B_ID);

        //Act
        var dto = new MeioPagamentoCadastroDto
        {
            Id = mp.Id,
            Nome = "Carteira Update",
            Tipo = TipoMeioPagamento.Carteira,
            Cor = "111111",
            Ordem = 1,
        };

        var response = await HttpClient.PostAsJsonAsync("/conta/alterar", dto);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.MeioPagamento_NaoPertenceAoUsuario);
    }

    [Fact]
    public async Task Alterar_CartaoTentaVirarConta_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var mp = await DbSeeder.InserirMeioPagamento(tipo: TipoMeioPagamento.CartaoCredito);

        //Act
        var dto = new MeioPagamentoCadastroDto
        {
            Id = mp.Id,
            Nome = "Conta Update",
            Tipo = TipoMeioPagamento.ContaCorrente,
            Cor = "111111",
            Limite = 5000,
            DiaFechamento = 1,
            DiaVencimento = 10
        };

        var response = await HttpClient.PostAsJsonAsync("/conta/alterar", dto);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.MeioPagamento_CartaoNaoPodeVirarConta);
    }

    [Fact]
    public async Task Alterar_ContaTentaVirarCartao_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var mp = await DbSeeder.InserirMeioPagamento(tipo: TipoMeioPagamento.ContaPagamento);

        //Act
        var dto = new MeioPagamentoCadastroDto
        {
            Id = mp.Id,
            Nome = "Conta Update",
            Tipo = TipoMeioPagamento.CartaoCredito,
            Cor = "111111"
        };

        var response = await HttpClient.PostAsJsonAsync("/conta/alterar", dto);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.MeioPagamento_ContaNaoPodeVirarCartao);
    }

    [Fact]
    public async Task Alterar_CartaoCredito_SemLimite_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var mp = await DbSeeder.InserirMeioPagamento(tipo: TipoMeioPagamento.CartaoCredito);

        //Act
        var dto = new MeioPagamentoCadastroDto
        {
            Id = mp.Id,
            Nome = "TestCard",
            Tipo = TipoMeioPagamento.CartaoCredito,
            Cor = "000000",
            Ordem = null,

            Limite = null,
            DiaFechamento = 1,
            DiaVencimento = 10
        };

        var response = await HttpClient.PostAsJsonAsync("/conta/alterar", dto);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.MeioPagamento_CamposObrigatorios);
    }

    [Fact]
    public async Task Alterar_CartaoCredito_SemDiaFechamento_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var mp = await DbSeeder.InserirMeioPagamento(tipo: TipoMeioPagamento.CartaoCredito);

        //Act
        var dto = new MeioPagamentoCadastroDto
        {
            Id = mp.Id,
            Nome = "TestCard",
            Tipo = TipoMeioPagamento.CartaoCredito,
            Cor = "000000",
            Ordem = null,

            Limite = 5000,
            DiaFechamento = null,
            DiaVencimento = 10
        };

        var response = await HttpClient.PostAsJsonAsync("/conta/alterar", dto);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.MeioPagamento_CamposObrigatorios);
    }

    [Fact]
    public async Task Alterar_CartaoCredito_SemDiaVencimento_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var mp = await DbSeeder.InserirMeioPagamento(tipo: TipoMeioPagamento.CartaoCredito);

        //Act
        var dto = new MeioPagamentoCadastroDto
        {
            Id = mp.Id,
            Nome = "TestCard",
            Tipo = TipoMeioPagamento.CartaoCredito,
            Cor = "000000",
            Ordem = null,

            Limite = 5000,
            DiaFechamento = 1,
            DiaVencimento = null
        };

        var response = await HttpClient.PostAsJsonAsync("/conta/alterar", dto);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.MeioPagamento_CamposObrigatorios);
    }

    [Fact]
    public async Task Alterar_DadosCorretos_NaoEhCartaoCredito_MeioPagamentoAlterado()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var mp = await DbSeeder.InserirMeioPagamento(tipo: TipoMeioPagamento.ContaCorrente);

        //Act
        var dto = new MeioPagamentoCadastroDto
        {
            Id = mp.Id,
            Nome = "Test Pay",
            Tipo = TipoMeioPagamento.ContaPagamento,
            Cor = "333333",
            Ordem = 4,
        };

        var response = await HttpClient.PostAsJsonAsync("/conta/alterar", dto);

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
        Assert.Equal(100, meio.Saldo);
        Assert.Null(meio.Limite);
        Assert.Null(meio.DiaFechamento);
        Assert.Null(meio.DiaVencimento);
    }

    [Fact]
    public async Task Alterar_DadosCorretosComCamposExtrasCartao_NaoEhCartaoCredito_MeioPagamentoAlterado_IgnoraCamposExtras()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var mp = await DbSeeder.InserirMeioPagamento(tipo: TipoMeioPagamento.ContaCorrente);

        //Act
        var dto = new MeioPagamentoCadastroDto
        {
            Id = mp.Id,
            Nome = "Test Pay",
            Tipo = TipoMeioPagamento.ContaPagamento,
            Cor = "333333",
            Ordem = 4,

            Limite = 10000,
            DiaFechamento = 1,
            DiaVencimento = 10
        };

        var response = await HttpClient.PostAsJsonAsync("/conta/alterar", dto);

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
        Assert.Equal(100, meio.Saldo);
        Assert.Null(meio.Limite);
        Assert.Null(meio.DiaFechamento);
        Assert.Null(meio.DiaVencimento);
    }

    [Fact]
    public async Task Alterar_DadosCorretos_EhCartaoCredito_MeioPagamentoAlterado()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var mp = await DbSeeder.InserirMeioPagamento(tipo: TipoMeioPagamento.CartaoCredito);

        //Act
        var dto = new MeioPagamentoCadastroDto
        {
            Id = mp.Id,
            Nome = "TestCard Gold",
            Tipo = TipoMeioPagamento.CartaoCredito,
            Cor = "444444",
            Ordem = 4,

            Limite = 8000,
            DiaFechamento = 2,
            DiaVencimento = 11
        };

        var response = await HttpClient.PostAsJsonAsync("/conta/alterar", dto);

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
        Assert.Equal(100, meio.Saldo);
        Assert.Equal(dto.Limite, meio.Limite);
        Assert.Equal(dto.DiaFechamento, meio.DiaFechamento);
        Assert.Equal(dto.DiaVencimento, meio.DiaVencimento);
    }
}
