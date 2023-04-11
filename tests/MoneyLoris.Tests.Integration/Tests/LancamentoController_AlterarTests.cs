using System.Net.Http.Json;
using MoneyLoris.Application.Business.Lancamentos.Dtos;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Shared;
using MoneyLoris.Tests.Integration.Setup.Utils;
using MoneyLoris.Tests.Integration.Tests.Base;

namespace MoneyLoris.Tests.Integration.Tests;
public class LancamentoController_AlterarTests : IntegrationTestsBase
{
    #region Validacoes

    [Fact]
    public async Task Alterar_UsuarioAdmin_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Administrador);
        await DbSeeder.InserirUsuarios();

        var cat = await DbSeeder.InserirCategoria();
        var mei = await DbSeeder.InserirMeioPagamento();
        var lanc = await DbSeeder.InserirLancamentoSimples(mei.Id, cat.Id);

        //Act
        var dto = new LancamentoCadastroDto
        {
            Id = lanc.Id,
            Data = SystemTime.Today(),
            Descricao = "Compras",
            Tipo = TipoLancamento.Despesa,
            IdCategoria = cat.Id,
            IdMeioPagamento = mei.Id,
            Valor = 10
        };

        var response = await HttpClient.PostAsJsonAsync("/lancamento/alterar", dto);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.Lancamento_AdminNaoPode);
    }

    [Fact]
    public async Task Alterar_MeioPagamentoNaoEncontrado_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var cat = await DbSeeder.InserirCategoria();
        var mei = await DbSeeder.InserirMeioPagamento();
        var lanc = await DbSeeder.InserirLancamentoSimples(mei.Id, cat.Id);

        //Act
        var dto = new LancamentoCadastroDto
        {
            Id = lanc.Id,
            Data = SystemTime.Today(),
            Descricao = "Compras",
            Tipo = TipoLancamento.Despesa,
            IdCategoria = cat.Id,
            IdMeioPagamento = 555,
            Valor = 10
        };

        var response = await HttpClient.PostAsJsonAsync("/lancamento/alterar", dto);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.MeioPagamento_NaoEncontrado);
    }

    [Fact]
    public async Task Alterar_MeioPagamentoNaoPertenceUsuario_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var cat = await DbSeeder.InserirCategoria();
        var mei = await DbSeeder.InserirMeioPagamento();
        var lanc = await DbSeeder.InserirLancamentoSimples(mei.Id, cat.Id);

        var meiB = await DbSeeder.InserirMeioPagamento(idUsuario: TestConstants.USUARIO_COMUM_B_ID);

        //Act
        var dto = new LancamentoCadastroDto
        {
            Id = lanc.Id,
            Data = SystemTime.Today(),
            Descricao = "Compras",
            Tipo = TipoLancamento.Despesa,
            IdCategoria = cat.Id,
            IdMeioPagamento = meiB.Id,
            Valor = 10
        };

        var response = await HttpClient.PostAsJsonAsync("/lancamento/alterar", dto);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.MeioPagamento_NaoPertenceAoUsuario);
    }

    [Fact]
    public async Task Alterar_MeioPagamentoTipoDiferente_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var cat = await DbSeeder.InserirCategoria();
        var mei = await DbSeeder.InserirMeioPagamento();
        var lanc = await DbSeeder.InserirLancamentoSimples(mei.Id, cat.Id);

        //Act
        var dto = new LancamentoCadastroDto
        {
            Id = lanc.Id,
            Data = SystemTime.Today(),
            Descricao = "Compras",
            Tipo = TipoLancamento.Receita,
            IdCategoria = cat.Id,
            IdMeioPagamento = mei.Id,
            Valor = 10
        };

        var response = await HttpClient.PostAsJsonAsync("/lancamento/alterar", dto);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.MeioPagamento_TipoDiferenteAlteracao);
    }

    #endregion
}
