using System.Net.Http.Json;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Shared;
using MoneyLoris.Tests.Integration.Setup.Utils;
using MoneyLoris.Tests.Integration.Tests.Base;

namespace MoneyLoris.Tests.Integration.Tests;
public class ContaController_ExcluirTests : IntegrationTestsBase
{
    [Fact]
    public async Task Excluir_UsuarioAdmin_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Administrador);
        await DbSeeder.InserirUsuarios();

        var mp = await DbSeeder.InserirMeioPagamento();

        //Act
        var response = await HttpClient.PostAsJsonAsync("/conta/excluir", mp.Id);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.MeioPagamento_AdminNaoPode);
    }

    [Fact]
    public async Task Excluir_MeioPagamentoNaoPertenceUsuario_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var mp = await DbSeeder.InserirMeioPagamento(idUsuario: TestConstants.USUARIO_COMUM_B_ID);

        //Act
        var response = await HttpClient.PostAsJsonAsync("/conta/excluir", mp.Id);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.MeioPagamento_NaoPertenceAoUsuario);
    }

    [Fact]
    public async Task Excluir_MeioPagamentoPossuiLancamento_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var mp = await DbSeeder.InserirMeioPagamento();
        var cat = await DbSeeder.InserirCategoria();
        var lanc = await DbSeeder.InserirLancamentoSimples(mp.Id, cat.Id);

        //Act
        var response = await HttpClient.PostAsJsonAsync("/conta/excluir", mp.Id);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.MeioPagamento_PossuiLancamentos);
    }

    [Fact]
    public async Task Excluir_DadosCorretos_MeioPagamentoExcluido()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var mp = await DbSeeder.InserirMeioPagamento();

        //Act
        var response = await HttpClient.PostAsJsonAsync("/conta/excluir", mp.Id);

        //Assert
        var idMeio = await response.ConverteResultOk<int>();

        var meio = await Context.MeiosPagamento.FindAsync(idMeio);

        Assert.Null(meio);
    }

}
