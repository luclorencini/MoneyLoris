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
    public async Task Alterar_MeioPagamentoDiferente_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var cat = await DbSeeder.InserirCategoria();
        var mei = await DbSeeder.InserirMeioPagamento();
        var lanc = await DbSeeder.InserirLancamentoSimples(mei.Id, cat.Id);

        var meiB = await DbSeeder.InserirMeioPagamento(nome: "Conta B");

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
            ErrorCodes.MeioPagamento_TipoDiferenteAlteracao);
    }

    [Fact]
    public async Task Alterar_LancamentoNaoPertenceUsuario_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var cat = await DbSeeder.InserirCategoria();
        var mei = await DbSeeder.InserirMeioPagamento();
        var lanc = await DbSeeder.InserirLancamentoSimples(mei.Id, cat.Id, idUsuario: TestConstants.USUARIO_COMUM_B_ID);

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
            ErrorCodes.Lancamento_NaoPertenceAoUsuario);
    }

    [Fact]
    public async Task Alterar_CategoriaNaoEncontrada_RetornaErro()
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
            IdCategoria = 555,
            IdMeioPagamento = mei.Id,
            Valor = 10
        };

        var response = await HttpClient.PostAsJsonAsync("/lancamento/alterar", dto);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.Categoria_NaoEncontrada);
    }

    [Fact]
    public async Task Alterar_CategoriaNaoPertenceUsuario_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var cat = await DbSeeder.InserirCategoria();
        var mei = await DbSeeder.InserirMeioPagamento();
        var lanc = await DbSeeder.InserirLancamentoSimples(mei.Id, cat.Id);

        var catB = await DbSeeder.InserirCategoria(idUsuario: TestConstants.USUARIO_COMUM_B_ID);

        //Act
        var dto = new LancamentoCadastroDto
        {
            Id = lanc.Id,
            Data = SystemTime.Today(),
            Descricao = "Compras",
            Tipo = TipoLancamento.Despesa,
            IdCategoria = catB.Id,
            IdMeioPagamento = mei.Id,
            Valor = 10
        };

        var response = await HttpClient.PostAsJsonAsync("/lancamento/alterar", dto);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.Categoria_NaoPertenceAoUsuario);
    }

    [Fact]
    public async Task Alterar_SubcategoriaNaoEncontrada_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var cat = await DbSeeder.InserirCategoria(inserirSubcategoria: true);
        var mei = await DbSeeder.InserirMeioPagamento();
        var lanc = await DbSeeder.InserirLancamentoSimples(mei.Id, cat.Id, cat.Subcategorias.First().Id);


        //Act
        var dto = new LancamentoCadastroDto
        {
            Id = lanc.Id,
            Data = SystemTime.Today(),
            Descricao = "Compras",
            Tipo = TipoLancamento.Despesa,
            IdCategoria = cat.Id,
            IdSubcategoria = 555,
            IdMeioPagamento = mei.Id,
            Valor = 10
        };

        var response = await HttpClient.PostAsJsonAsync("/lancamento/alterar", dto);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.Subcategoria_NaoEncontrada);
    }

    [Fact]
    public async Task Alterar_SubcategoriaNaPertenceCategoria_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var cat = await DbSeeder.InserirCategoria(inserirSubcategoria: true);
        var mei = await DbSeeder.InserirMeioPagamento();
        var lanc = await DbSeeder.InserirLancamentoSimples(mei.Id, cat.Id, cat.Subcategorias.First().Id);

        var catB = await DbSeeder.InserirCategoria(nome: "Diversos", inserirSubcategoria: true);

        //Act
        var dto = new LancamentoCadastroDto
        {
            Id = lanc.Id,
            Data = SystemTime.Today(),
            Descricao = "Compras",
            Tipo = TipoLancamento.Despesa,
            IdCategoria = cat.Id,
            IdSubcategoria = catB.Subcategorias.First().Id,
            IdMeioPagamento = mei.Id,
            Valor = 10
        };

        var response = await HttpClient.PostAsJsonAsync("/lancamento/alterar", dto);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.Subcategoria_NaoPertenceACategoria);
    }

    #endregion

    [Fact]
    public async Task Alterar_DadosCorretos_AlteraOQueForPermitido()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var cat = await DbSeeder.InserirCategoria(inserirSubcategoria: true);
        var mei = await DbSeeder.InserirMeioPagamento(saldo: 200);
        var lanc = await DbSeeder.InserirLancamentoSimples(mei.Id, cat.Id, cat.Subcategorias.First().Id, valor: -50);

        var catB = await DbSeeder.InserirCategoria(nome: "Diversos", inserirSubcategoria: true);

        //Act
        var dto = new LancamentoCadastroDto
        {
            Id = lanc.Id,
            Data = SystemTime.Today().AddDays(1),
            Descricao = "Gastos",
            Tipo = TipoLancamento.Despesa,
            IdCategoria = catB.Id,
            IdSubcategoria = catB.Subcategorias.First().Id,
            IdMeioPagamento = mei.Id,
            Valor = 10
        };

        var response = await HttpClient.PostAsJsonAsync("/lancamento/alterar", dto);

        //Assert
        var idLancamento = await response.ConverteResultOk<int>();

        Assert.True(idLancamento > 0);

        var ldb = await Context.Lancamentos.FindAsync(idLancamento);

        Assert.NotNull(lanc);
        Assert.Equal(TestConstants.USUARIO_COMUM_ID, ldb!.IdUsuario);
        Assert.Equal(dto.IdMeioPagamento, ldb.IdMeioPagamento);
        Assert.Equal(TipoLancamento.Despesa, ldb.Tipo);
        Assert.Equal(OperacaoLancamento.LancamentoSimples, ldb.Operacao);
        Assert.Null(lanc.TipoTransferencia);
        Assert.Equal(dto.Data, ldb!.Data);
        Assert.Equal(dto.Descricao, ldb.Descricao);
        Assert.Equal(dto.IdCategoria, ldb.IdCategoria);
        Assert.Equal(dto.IdSubcategoria, ldb.IdSubcategoria);
        Assert.True(ldb.Realizado);
        Assert.Null(ldb.IdLancamentoTransferencia);
        Assert.Equal(-50, ldb.Valor);  //valor ainda não pode mudar

        var conta = await Context.MeiosPagamento.FindAsync(mei.Id);

        Assert.NotNull(conta);
        Assert.Equal(TestConstants.USUARIO_COMUM_ID, conta!.IdUsuario);
        Assert.Equal(200, conta.Saldo);  //se o valor nao muda, o saldo tambem nao
    }

}
