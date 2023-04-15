using MoneyLoris.Application.Business.MeiosPagamento.Dtos;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Shared;
using MoneyLoris.Tests.Integration.Setup.Utils;
using MoneyLoris.Tests.Integration.Tests.Base;

namespace MoneyLoris.Tests.Integration.Tests.MeiosPagamento;
public class ContaController_ConsultaTests : IntegrationTestsBase
{
    private async Task setupDadosListagem()
    {
        await DbSeeder.InserirMeioPagamento(TestConstants.USUARIO_COMUM_ID, TipoMeioPagamento.ContaCorrente, "Caixa", 3);
        await DbSeeder.InserirMeioPagamento(TestConstants.USUARIO_COMUM_ID, TipoMeioPagamento.ContaPagamento, "Picpay", 2);
        await DbSeeder.InserirMeioPagamento(TestConstants.USUARIO_COMUM_ID, TipoMeioPagamento.CartaoCredito, "Neon", null);
        await DbSeeder.InserirMeioPagamento(TestConstants.USUARIO_COMUM_ID, TipoMeioPagamento.Carteira, "Carteira", 3);
        await DbSeeder.InserirMeioPagamento(TestConstants.USUARIO_COMUM_ID, TipoMeioPagamento.CartaoBeneficio, "Sodexo", null);
        await DbSeeder.InserirMeioPagamento(TestConstants.USUARIO_COMUM_ID, TipoMeioPagamento.CartaoCredito, "Will Bank", null);
        await DbSeeder.InserirMeioPagamento(TestConstants.USUARIO_COMUM_ID, TipoMeioPagamento.Poupanca, "Itaú", null);
        await DbSeeder.InserirMeioPagamento(TestConstants.USUARIO_COMUM_ID, TipoMeioPagamento.CarteiraDigital, "NuConta", 1);
        await DbSeeder.InserirMeioPagamento(TestConstants.USUARIO_COMUM_B_ID, TipoMeioPagamento.ContaCorrente, "Santander", null);
        await DbSeeder.InserirMeioPagamento(TestConstants.USUARIO_COMUM_B_ID, TipoMeioPagamento.CartaoCredito, "Banco Inter", null);
    }

    [Fact]
    public async Task Obter_NaoExisteNaBase_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        //Act
        var response = await HttpClient.GetAsync("/conta/obter/1");

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.MeioPagamento_NaoEncontrado);
    }

    [Fact]
    public async Task Obter_BuscaMeioPagamentoDeOutroUsuario_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var mp = await DbSeeder.InserirMeioPagamento(TestConstants.USUARIO_COMUM_B_ID);

        //Act
        var response = await HttpClient.GetAsync($"/conta/obter/{mp.Id}");

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.MeioPagamento_NaoPertenceAoUsuario);
    }

    [Fact]
    public async Task Obter_NaoEhCredito_DadosNaBase_RetornaMeioPagamento()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var meio = await DbSeeder.InserirMeioPagamento(TestConstants.USUARIO_COMUM_ID, TipoMeioPagamento.ContaCorrente, "Santander", 1);

        //Act
        var response = await HttpClient.GetAsync($"/conta/obter/{meio.Id}");

        //Assert
        var dto = await response.ConverteResultOk<MeioPagamentoCadastroDto>();

        Assert.NotNull(dto);
        Assert.Equal(meio.Id, dto!.Id);
        Assert.Equal(meio.Nome, dto.Nome);
        Assert.Equal(meio.Tipo, dto.Tipo);
        Assert.Equal(meio.Cor, dto.Cor);
        Assert.Equal(meio.Ordem, dto.Ordem);
        Assert.True(dto.Ativo);
        Assert.Null(dto.Limite);
        Assert.Null(dto.DiaFechamento);
        Assert.Null(dto.DiaVencimento);
    }

    [Fact]
    public async Task Obter_EhCredito_DadosNaBase_RetornaMeioPagamento()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var meio = await DbSeeder.InserirMeioPagamento(TestConstants.USUARIO_COMUM_ID, TipoMeioPagamento.CartaoCredito, "Nubank", 2);

        //Act
        var response = await HttpClient.GetAsync($"/conta/obter/{meio.Id}");

        //Assert
        var dto = await response.ConverteResultOk<MeioPagamentoCadastroDto>();

        Assert.NotNull(dto);
        Assert.Equal(meio.Id, dto!.Id);
        Assert.Equal(meio.Nome, dto.Nome);
        Assert.Equal(meio.Tipo, dto.Tipo);
        Assert.Equal(meio.Cor, dto.Cor);
        Assert.Equal(meio.Ordem, dto.Ordem);
        Assert.True(dto.Ativo);

        Assert.Equal(5000, dto.Limite);
        Assert.Equal((byte)1, dto.DiaFechamento);
        Assert.Equal((byte)10, dto.DiaVencimento);
    }



    [Fact]
    public async Task Listar_DadosNaBase_RetornaLista()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        await setupDadosListagem();

        //Act
        var response = await HttpClient.GetAsync("/conta/listar");

        //Assert
        var dto = await response.ConverteResultOk<ICollection<MeioPagamentoCadastroListItemDto>>();

        Assert.NotNull(dto);
        Assert.Equal(8, dto.Count);

        var arr = dto.ToArray();

        Assert.Equal("NuConta", arr[0].Nome);
        Assert.Equal("Picpay", arr[1].Nome);
        Assert.Equal("Caixa", arr[2].Nome);
        Assert.Equal("Carteira", arr[3].Nome);
        Assert.Equal("Itaú", arr[4].Nome);
        Assert.Equal("Neon", arr[5].Nome);
        Assert.Equal("Sodexo", arr[6].Nome);
        Assert.Equal("Will Bank", arr[7].Nome);
    }

    [Fact]
    public async Task Common_MeiosPagamento_DadosNaBase_RetornaLista()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        await setupDadosListagem();

        //Act
        var response = await HttpClient.GetAsync("/common/meiosPagamento");

        //Assert
        var dto = await response.ConverteResultOk<ICollection<MeioPagamentoCadastroListItemDto>>();

        Assert.NotNull(dto);
        Assert.Equal(8, dto.Count);

        var arr = dto.ToArray();

        Assert.Equal("NuConta", arr[0].Nome);
        Assert.Equal("Picpay", arr[1].Nome);
        Assert.Equal("Caixa", arr[2].Nome);
        Assert.Equal("Carteira", arr[3].Nome);
        Assert.Equal("Itaú", arr[4].Nome);
        Assert.Equal("Neon", arr[5].Nome);
        Assert.Equal("Sodexo", arr[6].Nome);
        Assert.Equal("Will Bank", arr[7].Nome);
    }

    [Fact]
    public async Task Common_Contas_DadosNaBase_RetornaLista()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        await setupDadosListagem();

        //Act
        var response = await HttpClient.GetAsync("/common/contas");

        //Assert
        var dto = await response.ConverteResultOk<ICollection<MeioPagamentoCadastroListItemDto>>();

        Assert.NotNull(dto);
        Assert.Equal(6, dto.Count);

        var arr = dto.ToArray();

        Assert.Equal("NuConta", arr[0].Nome);
        Assert.Equal("Picpay", arr[1].Nome);
        Assert.Equal("Caixa", arr[2].Nome);
        Assert.Equal("Carteira", arr[3].Nome);
        Assert.Equal("Itaú", arr[4].Nome);
        Assert.Equal("Sodexo", arr[5].Nome);
    }

    [Fact]
    public async Task Common_Cartoes_DadosNaBase_RetornaLista()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        await setupDadosListagem();

        //Act
        var response = await HttpClient.GetAsync("/common/cartoes");

        //Assert
        var dto = await response.ConverteResultOk<ICollection<MeioPagamentoCadastroListItemDto>>();

        Assert.NotNull(dto);
        Assert.Equal(2, dto.Count);

        var arr = dto.ToArray();

        Assert.Equal("Neon", arr[0].Nome);
        Assert.Equal("Will Bank", arr[1].Nome);
    }
}
