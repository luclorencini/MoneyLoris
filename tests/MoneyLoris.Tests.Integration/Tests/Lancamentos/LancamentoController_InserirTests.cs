using System.Net.Http.Json;
using MoneyLoris.Application.Business.Lancamentos.Dtos;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Shared;
using MoneyLoris.Tests.Integration.Setup.Utils;
using MoneyLoris.Tests.Integration.Tests.Base;

namespace MoneyLoris.Tests.Integration.Tests.Lancamentos;
public class LancamentoController_InserirTests : IntegrationTestsBase
{
    #region Validacoes

    [Fact]
    public async Task LancarReceita_UsuarioAdmin_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Administrador);
        await DbSeeder.InserirUsuarios();

        var cat = await DbSeeder.InserirCategoria();
        var mei = await DbSeeder.InserirMeioPagamento();

        //Act
        var dto = new LancamentoCadastroDto
        {
            Data = SystemTime.Today(),
            Descricao = "Compras",
            Tipo = TipoLancamento.Receita,
            IdCategoria = cat.Id,
            IdMeioPagamento = mei.Id,
            Valor = 10
        };

        var response = await HttpClient.PostAsJsonAsync("/lancamento/lancar/receita", dto);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.Lancamento_AdminNaoPode);
    }

    [Fact]
    public async Task LancarDespesa_MeioPagamentoNaoEncontrado_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var cat = await DbSeeder.InserirCategoria();

        //Act
        var dto = new LancamentoCadastroDto
        {
            Data = SystemTime.Today(),
            Descricao = "Compras",
            Tipo = TipoLancamento.Despesa,
            IdCategoria = cat.Id,
            IdMeioPagamento = 555,
            Valor = 10
        };

        var response = await HttpClient.PostAsJsonAsync("/lancamento/lancar/despesa", dto);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.MeioPagamento_NaoEncontrado);
    }

    [Fact]
    public async Task LancarReceita_MeioPagamentoNaoPertenceUsuario_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var cat = await DbSeeder.InserirCategoria();
        var mei = await DbSeeder.InserirMeioPagamento(idUsuario: TestConstants.USUARIO_COMUM_B_ID);

        //Act
        var dto = new LancamentoCadastroDto
        {
            Data = SystemTime.Today(),
            Descricao = "Compras",
            Tipo = TipoLancamento.Receita,
            IdCategoria = cat.Id,
            IdMeioPagamento = mei.Id,
            Valor = 10
        };

        var response = await HttpClient.PostAsJsonAsync("/lancamento/lancar/receita", dto);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.MeioPagamento_NaoPertenceAoUsuario);
    }

    [Fact]
    public async Task LancarDespesa_MeioPagamentoInativo_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var cat = await DbSeeder.InserirCategoria();
        var mei = await DbSeeder.InserirMeioPagamento(ativo: false);

        //Act
        var dto = new LancamentoCadastroDto
        {
            Data = SystemTime.Today(),
            Descricao = "Compras",
            Tipo = TipoLancamento.Despesa,
            IdCategoria = cat.Id,
            IdMeioPagamento = mei.Id,
            Valor = 10
        };

        var response = await HttpClient.PostAsJsonAsync("/lancamento/lancar/despesa", dto);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.MeioPagamento_Inativo);
    }

    [Fact]
    public async Task LancarReceita_CategoriaNaoEncontrada_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var mei = await DbSeeder.InserirMeioPagamento();

        //Act
        var dto = new LancamentoCadastroDto
        {
            Data = SystemTime.Today(),
            Descricao = "Compras",
            Tipo = TipoLancamento.Receita,
            IdCategoria = 555,
            IdMeioPagamento = mei.Id,
            Valor = 10
        };

        var response = await HttpClient.PostAsJsonAsync("/lancamento/lancar/receita", dto);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.Categoria_NaoEncontrada);
    }

    [Fact]
    public async Task LancarDespesa_CategoriaNaoPertenceUsuario_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var cat = await DbSeeder.InserirCategoria(idUsuario: TestConstants.USUARIO_COMUM_B_ID);
        var mei = await DbSeeder.InserirMeioPagamento();

        //Act
        var dto = new LancamentoCadastroDto
        {
            Data = SystemTime.Today(),
            Descricao = "Compras",
            Tipo = TipoLancamento.Despesa,
            IdCategoria = cat.Id,
            IdMeioPagamento = mei.Id,
            Valor = 10
        };

        var response = await HttpClient.PostAsJsonAsync("/lancamento/lancar/despesa", dto);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.Categoria_NaoPertenceAoUsuario);
    }

    [Fact]
    public async Task LancarReceita_CategoriaTipoDespesa_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var cat = await DbSeeder.InserirCategoria(TipoLancamento.Despesa);
        var mei = await DbSeeder.InserirMeioPagamento();

        //Act
        var dto = new LancamentoCadastroDto
        {
            Data = SystemTime.Today(),
            Descricao = "Compras",
            Tipo = TipoLancamento.Receita,
            IdCategoria = cat.Id,
            IdMeioPagamento = mei.Id,
            Valor = 10
        };

        var response = await HttpClient.PostAsJsonAsync("/lancamento/lancar/receita", dto);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.Lancamento_TipoDiferenteDaCategoria);
    }

    [Fact]
    public async Task LancarDespesa_CategoriaTipoReceita_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var cat = await DbSeeder.InserirCategoria(TipoLancamento.Receita);
        var mei = await DbSeeder.InserirMeioPagamento();

        //Act
        var dto = new LancamentoCadastroDto
        {
            Data = SystemTime.Today(),
            Descricao = "Compras",
            Tipo = TipoLancamento.Despesa,
            IdCategoria = cat.Id,
            IdMeioPagamento = mei.Id,
            Valor = 10
        };

        var response = await HttpClient.PostAsJsonAsync("/lancamento/lancar/despesa", dto);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.Lancamento_TipoDiferenteDaCategoria);
    }

    #endregion

    [Fact]
    public async Task LancarReceita_DadosCertos_MeioNaoEhCartao_LancamentoRealizado_SaldoMeioAtualizado()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var cat = await DbSeeder.InserirCategoria(TipoLancamento.Receita);
        var mei = await DbSeeder.InserirMeioPagamento(saldo: 200, tipo: TipoMeioPagamento.ContaCorrente);

        //Act
        var dto = new LancamentoCadastroDto
        {
            Data = SystemTime.Today(),
            Descricao = "Compras",
            Tipo = TipoLancamento.Receita,
            IdCategoria = cat.Id,
            IdMeioPagamento = mei.Id,
            Valor = 10
        };

        var response = await HttpClient.PostAsJsonAsync("/lancamento/lancar/receita", dto);

        //Assert
        var idLancamento = await response.ConverteResultOk<int>();

        Assert.True(idLancamento > 0);

        var lanc = await Context.Lancamentos.FindAsync(idLancamento);

        Assert.NotNull(lanc);
        Assert.Equal(TestConstants.USUARIO_COMUM_ID, lanc!.IdUsuario);
        Assert.Equal(dto.IdMeioPagamento, lanc.IdMeioPagamento);
        Assert.Equal(dto.Tipo, lanc.Tipo);
        Assert.Equal(OperacaoLancamento.LancamentoSimples, lanc.Operacao);
        Assert.Null(lanc.TipoTransferencia);
        Assert.Equal(dto.Data, lanc!.Data);
        Assert.Equal(dto.Descricao, lanc.Descricao);
        Assert.Equal(10, lanc.Valor);
        Assert.Equal(dto.IdCategoria, lanc.IdCategoria);
        Assert.Null(lanc.IdSubcategoria);
        Assert.True(lanc.Realizado);
        Assert.Null(lanc.IdLancamentoTransferencia);

        var conta = await Context.MeiosPagamento.FindAsync(mei.Id);

        Assert.NotNull(conta);
        Assert.Equal(TestConstants.USUARIO_COMUM_ID, conta!.IdUsuario);
        Assert.Equal(210, conta.Saldo);
    }

    [Fact]
    public async Task LancarDespesa_DadosCertos_MeioEhCartao_LancamentoRealizado_SaldoMeioNaoAtualiza()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var cat = await DbSeeder.InserirCategoria(TipoLancamento.Despesa);
        var sub = await DbSeeder.InserirSubcategoria(cat.Id);
        var mei = await DbSeeder.InserirMeioPagamento(saldo: 0, tipo: TipoMeioPagamento.CartaoCredito);

        //Act
        var dto = new LancamentoCadastroDto
        {
            Data = SystemTime.Today(),
            Descricao = "Compras",
            Tipo = TipoLancamento.Despesa,
            IdCategoria = cat.Id,
            IdSubcategoria = sub.Id,
            IdMeioPagamento = mei.Id,
            Valor = 10
        };

        var response = await HttpClient.PostAsJsonAsync("/lancamento/lancar/despesa", dto);

        //Assert
        var idLancamento = await response.ConverteResultOk<int>();

        Assert.True(idLancamento > 0);

        var lanc = await Context.Lancamentos.FindAsync(idLancamento);

        Assert.NotNull(lanc);
        Assert.Equal(TestConstants.USUARIO_COMUM_ID, lanc!.IdUsuario);
        Assert.Equal(dto.IdMeioPagamento, lanc.IdMeioPagamento);
        Assert.Equal(dto.Tipo, lanc.Tipo);
        Assert.Equal(OperacaoLancamento.LancamentoSimples, lanc.Operacao);
        Assert.Null(lanc.TipoTransferencia);
        Assert.Equal(dto.Data, lanc!.Data);
        Assert.Equal(dto.Descricao, lanc.Descricao);
        Assert.Equal(-10, lanc.Valor);
        Assert.Equal(dto.IdCategoria, lanc.IdCategoria);
        Assert.Equal(dto.IdSubcategoria, lanc.IdSubcategoria);
        Assert.True(lanc.Realizado);
        Assert.Null(lanc.IdLancamentoTransferencia);

        var conta = await Context.MeiosPagamento.FindAsync(mei.Id);

        Assert.NotNull(conta);
        Assert.Equal(TestConstants.USUARIO_COMUM_ID, conta!.IdUsuario);
        Assert.Equal(0, conta.Saldo);
    }
}
