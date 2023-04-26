using System.Net.Http.Json;
using MoneyLoris.Application.Business.Lancamentos.Dtos;
using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Shared;
using MoneyLoris.Tests.Integration.Setup.Utils;
using MoneyLoris.Tests.Integration.Tests.Base;

namespace MoneyLoris.Tests.Integration.Tests.Lancamentos;
public class LancamentoController_ExcluirTests : IntegrationTestsBase
{

    #region Validacoes

    [Fact]
    public async Task Excluir_UsuarioAdmin_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Administrador);
        await DbSeeder.InserirUsuarios();

        var cat = await DbSeeder.InserirCategoria();
        var mei = await DbSeeder.InserirMeioPagamento();
        var lanc = await DbSeeder.InserirLancamentoSimples(mei.Id, cat.Id);

        //Act
        var response = await HttpClient.PostAsJsonAsync("/lancamento/excluir", lanc.Id);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.Lancamento_AdminNaoPode);
    }

    [Fact]
    public async Task Excluir_LancamentoNaoEncontrado_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        //Act
        var response = await HttpClient.PostAsJsonAsync("/lancamento/excluir", 555);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.Lancamento_NaoEncontrado);
    }

    [Fact]
    public async Task Excluir_LancamentoNaoPertenceUsuario_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var cat = await DbSeeder.InserirCategoria();
        var mei = await DbSeeder.InserirMeioPagamento();
        var lanc = await DbSeeder.InserirLancamentoSimples(mei.Id, cat.Id, idUsuario: TestConstants.USUARIO_COMUM_B_ID);

        //Act
        var response = await HttpClient.PostAsJsonAsync("/lancamento/excluir", lanc.Id);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.Lancamento_NaoPertenceAoUsuario);
    }

    [Fact]
    public async Task Excluir_MeioPagamentoNaoPertenceUsuario_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var cat = await DbSeeder.InserirCategoria();
        var mei = await DbSeeder.InserirMeioPagamento(idUsuario: TestConstants.USUARIO_COMUM_B_ID);
        var lanc = await DbSeeder.InserirLancamentoSimples(mei.Id, cat.Id);

        //Act
        var response = await HttpClient.PostAsJsonAsync("/lancamento/excluir", lanc.Id);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.MeioPagamento_NaoPertenceAoUsuario);
    }

    #endregion

    [Fact]
    public async Task Excluir_MeioNaoEhCartao_LancamentoExcluido_SaldoMeioAtualizado()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var cat = await DbSeeder.InserirCategoria(TipoLancamento.Receita);
        var mei = await DbSeeder.InserirMeioPagamento(saldo: 200, tipo: TipoMeioPagamento.ContaCorrente);
        var lan = await DbSeeder.InserirLancamentoSimples(mei.Id, cat.Id, valor: -50, tipo: TipoLancamento.Despesa);

        //Act
        var response = await HttpClient.PostAsJsonAsync("/lancamento/excluir", lan.Id);

        //Assert
        var idLancamento = await response.ConverteResultOk<int>();

        Assert.True(idLancamento > 0);

        var lanc = await Context.Lancamentos.FindAsync(idLancamento);

        Assert.Null(lanc);

        var conta = await Context.MeiosPagamento.FindAsync(mei.Id);

        Assert.NotNull(conta);
        Assert.Equal(TestConstants.USUARIO_COMUM_ID, conta!.IdUsuario);
        Assert.Equal(250, conta.Saldo);
    }

    [Fact]
    public async Task Excluir_MeioEhCartaoCredito_LancamentoExcluido_SaldoMeioNaoAtualizado()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var cat = await DbSeeder.InserirCategoria(TipoLancamento.Receita);
        var mei = await DbSeeder.InserirMeioPagamento(saldo: 0, tipo: TipoMeioPagamento.CartaoCredito);
        var lan = await DbSeeder.InserirLancamentoSimples(mei.Id, cat.Id, valor: -50, tipo: TipoLancamento.Despesa);

        //Act
        var response = await HttpClient.PostAsJsonAsync("/lancamento/excluir", lan.Id);

        //Assert
        var idLancamento = await response.ConverteResultOk<int>();

        Assert.True(idLancamento > 0);

        var lanc = await Context.Lancamentos.FindAsync(idLancamento);

        Assert.Null(lanc);

        var conta = await Context.MeiosPagamento.FindAsync(mei.Id);

        Assert.NotNull(conta);
        Assert.Equal(TestConstants.USUARIO_COMUM_ID, conta!.IdUsuario);
        Assert.Equal(0, conta.Saldo);
    }
}