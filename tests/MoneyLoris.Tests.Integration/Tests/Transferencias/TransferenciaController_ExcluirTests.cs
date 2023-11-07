using System.Net.Http.Json;
using K4os.Compression.LZ4.Internal;
using MoneyLoris.Application.Business.Lancamentos.Dtos;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Shared;
using MoneyLoris.Tests.Integration.Setup.Utils;
using MoneyLoris.Tests.Integration.Tests.Base;

namespace MoneyLoris.Tests.Integration.Tests.Transferencias;
public class TransferenciaController_ExcluirTests : IntegrationTestsBase
{
    #region Validacoes

    [Fact]
    public async Task Excluir_UsuarioAdmin_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Administrador);
        await DbSeeder.InserirUsuarios();

        var mOri = await DbSeeder.InserirMeioPagamento(nome: "Conta Ori", saldo: 400);
        var mDes = await DbSeeder.InserirMeioPagamento(nome: "Conta Des", saldo: 200);

        var lOri = await DbSeeder.InserirLancamentoTransferencia(mOri.Id);
        var lDes = await DbSeeder.InserirLancamentoTransferencia(mDes.Id);

        await DbSeeder.AssociarLancamentosTransferencia(lOri, lDes);

        //Act
        var response = await HttpClient.PostAsJsonAsync("/transferencia/excluir", lOri.Id);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.Transferencia_AdminNaoPode);
    }

    [Fact]
    public async Task Excluir_LancamentoOrigemNaoEncontrado_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        //Act
        var response = await HttpClient.PostAsJsonAsync("/transferencia/excluir", 555);

        //Assert
        var ret = await response.AssertResultNotOk(
            ErrorCodes.Lancamento_OrigemNaoEncontrado);
    }

    [Fact]
    public async Task Excluir_LancamentoOrigemNaoPertenceUsuario_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var mOri = await DbSeeder.InserirMeioPagamento(nome: "Conta Ori", saldo: 400, idUsuario: TestConstants.USUARIO_COMUM_B_ID);
        var mDes = await DbSeeder.InserirMeioPagamento(nome: "Conta Des", saldo: 200);

        var lOri = await DbSeeder.InserirLancamentoTransferencia(mOri.Id, idUsuario: TestConstants.USUARIO_COMUM_B_ID);
        var lDes = await DbSeeder.InserirLancamentoTransferencia(mDes.Id);

        await DbSeeder.AssociarLancamentosTransferencia(lOri, lDes);

        //Act
        var response = await HttpClient.PostAsJsonAsync("/transferencia/excluir", lOri.Id);

        //Assert
        var ret = await response.AssertResultNotOk(
            ErrorCodes.Lancamento_OrigemNaoPertenceAoUsuario);
    }

    [Fact]
    public async Task Excluir_LancamentoOrigemOperacaoNaoEhTransferencia_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var cat = await DbSeeder.InserirCategoria();
        var mOri = await DbSeeder.InserirMeioPagamento(nome: "Conta Ori", saldo: 400);
        var mDes = await DbSeeder.InserirMeioPagamento(nome: "Conta Des", saldo: 200);

        var lOri = await DbSeeder.InserirLancamentoSimples(mOri.Id, cat.Id);
        var lDes = await DbSeeder.InserirLancamentoTransferencia(mDes.Id);

        await DbSeeder.AssociarLancamentosTransferencia(lOri, lDes);

        //Act
        var response = await HttpClient.PostAsJsonAsync("/transferencia/excluir", lOri.Id);

        //Assert
        var ret = await response.AssertResultNotOk(
            ErrorCodes.Transferencia_OperacaoLancamentoOrigemNaoEhTransferencia);
    }

    [Fact]
    public async Task Excluir_LancamentoDestinoNaoPertenceUsuario_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var mOri = await DbSeeder.InserirMeioPagamento(nome: "Conta Ori", saldo: 400);
        var mDes = await DbSeeder.InserirMeioPagamento(nome: "Conta Des", saldo: 200, idUsuario: TestConstants.USUARIO_COMUM_B_ID);

        var lOri = await DbSeeder.InserirLancamentoTransferencia(mOri.Id);
        var lDes = await DbSeeder.InserirLancamentoTransferencia(mDes.Id, idUsuario: TestConstants.USUARIO_COMUM_B_ID);

        await DbSeeder.AssociarLancamentosTransferencia(lOri, lDes);

        //Act
        var response = await HttpClient.PostAsJsonAsync("/transferencia/excluir", lOri.Id);

        //Assert
        var ret = await response.AssertResultNotOk(
            ErrorCodes.Lancamento_DestinoNaoPertenceAoUsuario);
    }

    [Fact]
    public async Task Excluir_LancamentoDestinoOperacaoNaoEhTransferencia_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var cat = await DbSeeder.InserirCategoria();
        var mOri = await DbSeeder.InserirMeioPagamento(nome: "Conta Ori", saldo: 400);
        var mDes = await DbSeeder.InserirMeioPagamento(nome: "Conta Des", saldo: 200);

        var lOri = await DbSeeder.InserirLancamentoTransferencia(mOri.Id);
        var lDes = await DbSeeder.InserirLancamentoSimples(mDes.Id, cat.Id);

        await DbSeeder.AssociarLancamentosTransferencia(lOri, lDes);

        //Act
        var response = await HttpClient.PostAsJsonAsync("/transferencia/excluir", lOri.Id);

        //Assert
        var ret = await response.AssertResultNotOk(
            ErrorCodes.Transferencia_OperacaoLancamentoDestinoNaoEhTransferencia);
    }

    //[Fact]
    //public async Task Excluir_MeioPagamentoOrigemNaoEncontrado_RetornaErro()
    //{
    //}

    [Fact]
    public async Task Excluir_MeioPagamentoOrigemNaoPertenceUsuario_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var mOri = await DbSeeder.InserirMeioPagamento(nome: "Conta Ori", saldo: 400, idUsuario: TestConstants.USUARIO_COMUM_B_ID);
        var mDes = await DbSeeder.InserirMeioPagamento(nome: "Conta Des", saldo: 200);

        var lOri = await DbSeeder.InserirLancamentoTransferencia(mOri.Id);
        var lDes = await DbSeeder.InserirLancamentoTransferencia(mDes.Id);

        await DbSeeder.AssociarLancamentosTransferencia(lOri, lDes);

        //Act
        var response = await HttpClient.PostAsJsonAsync("/transferencia/excluir", lOri.Id);

        //Assert
        var ret = await response.AssertResultNotOk(
            ErrorCodes.MeioPagamento_OrigemNaoPertenceAoUsuario);
    }

    [Fact]
    public async Task Excluir_MeioPagamentoOrigemEhCartaoCredito_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var mOri = await DbSeeder.InserirMeioPagamento(nome: "Conta Ori", saldo: 400, tipo: TipoMeioPagamento.CartaoCredito);
        var mDes = await DbSeeder.InserirMeioPagamento(nome: "Cartão Des", saldo: 0, tipo: TipoMeioPagamento.CartaoCredito);

        var lOri = await DbSeeder.InserirLancamentoTransferencia(mOri.Id);
        var lDes = await DbSeeder.InserirLancamentoTransferencia(mDes.Id);

        await DbSeeder.AssociarLancamentosTransferencia(lOri, lDes);

        //Act
        var response = await HttpClient.PostAsJsonAsync("/transferencia/excluir", lOri.Id);

        //Assert
        var ret = await response.AssertResultNotOk(
            ErrorCodes.Transferencia_MeioOrigemNaoPodeSerCartao);
    }

    //[Fact]
    //public async Task Excluir_MeioPagamentoDestinoNaoEncontrado_RetornaErro()
    //{
    //}

    [Fact]
    public async Task Excluir_MeioPagamentoDestinoNaoPertenceUsuario_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var mOri = await DbSeeder.InserirMeioPagamento(nome: "Conta Ori", saldo: 400);
        var mDes = await DbSeeder.InserirMeioPagamento(nome: "Conta Des", saldo: 200, idUsuario: TestConstants.USUARIO_COMUM_B_ID);

        var lOri = await DbSeeder.InserirLancamentoTransferencia(mOri.Id);
        var lDes = await DbSeeder.InserirLancamentoTransferencia(mDes.Id);

        await DbSeeder.AssociarLancamentosTransferencia(lOri, lDes);

        //Act
        var response = await HttpClient.PostAsJsonAsync("/transferencia/excluir", lOri.Id);

        //Assert
        var ret = await response.AssertResultNotOk(
            ErrorCodes.MeioPagamento_DestinoNaoPertenceAoUsuario);
    }

    #endregion

    [Fact]
    public async Task Excluir_EntreContas_DadosCertos_TransferenciaExcluida_SaldosAtualizados()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var mOri = await DbSeeder.InserirMeioPagamento(nome: "Conta Ori", saldo: 400);
        var mDes = await DbSeeder.InserirMeioPagamento(nome: "Conta Des", saldo: 200);

        var lOri = await DbSeeder.InserirLancamentoTransferencia(mOri.Id, valor: -50, tipo: TipoLancamento.Despesa);
        var lDes = await DbSeeder.InserirLancamentoTransferencia(mDes.Id, valor: 50, tipo: TipoLancamento.Receita);

        await DbSeeder.AssociarLancamentosTransferencia(lOri, lDes);

        //Act
        var response = await HttpClient.PostAsJsonAsync("/transferencia/excluir", lOri.Id);

        //Assert

        //origem
        var idLancOri = await response.ConverteResultOk<int>();

        Assert.True(idLancOri > 0);

        var lancOri = await Context.Lancamentos.FindAsync(idLancOri);
        Assert.Null(lancOri);

        var contaOri = await Context.MeiosPagamento.FindAsync(mOri.Id);

        Assert.NotNull(contaOri);
        Assert.Equal(TestConstants.USUARIO_COMUM_ID, contaOri!.IdUsuario);
        Assert.Equal(450, contaOri.Saldo);

        //destino
        var idLancDes = lDes.Id;

        Assert.True(idLancDes > 0);

        var lancDes = await Context.Lancamentos.FindAsync(idLancDes);
        Assert.Null(lancDes);

        var contaDes = await Context.MeiosPagamento.FindAsync(mDes.Id);

        Assert.NotNull(contaDes);
        Assert.Equal(TestConstants.USUARIO_COMUM_ID, contaDes!.IdUsuario);
        Assert.Equal(150, contaDes.Saldo);
    }

    [Fact]
    public async Task Excluir_PagamentoFatura_DadosCertos_TransferenciaExcluida_SaldoMeioOrigemAtualiza_SaldoMeioDestinoNaoAtualiza_FaturaDiminuiValorPago()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var mOri = await DbSeeder.InserirMeioPagamento(nome: "Conta Ori", saldo: 600);
        var mDes = await DbSeeder.InserirMeioPagamento(nome: "Conta Des", saldo: 0, tipo: TipoMeioPagamento.CartaoCredito);

        var fat = await DbSeeder.InserirFatura(mDes.Id, 11, 2023, valorPago: 90);

        var lOri = await DbSeeder.InserirLancamentoTransferencia(mOri.Id, valor: -50, tipo: TipoLancamento.Despesa, tipoTransferencia: TipoTransferencia.PagamentoFatura);
        var lDes = await DbSeeder.InserirLancamentoTransferencia(mDes.Id, valor: 50, tipo: TipoLancamento.Receita, tipoTransferencia: TipoTransferencia.PagamentoFatura, idFatura: fat.Id);

        await DbSeeder.AssociarLancamentosTransferencia(lOri, lDes);

        //Act
        var response = await HttpClient.PostAsJsonAsync("/transferencia/excluir", lOri.Id);

        //Assert

        //origem
        var idLancOri = await response.ConverteResultOk<int>();

        Assert.True(idLancOri > 0);

        var lancOri = await Context.Lancamentos.FindAsync(idLancOri);
        Assert.Null(lancOri);

        var contaOri = await Context.MeiosPagamento.FindAsync(mOri.Id);

        Assert.NotNull(contaOri);
        Assert.Equal(TestConstants.USUARIO_COMUM_ID, contaOri!.IdUsuario);
        Assert.Equal(650, contaOri.Saldo);

        //destino
        var idLancDes = lDes.Id;

        Assert.True(idLancDes > 0);

        var lancDes = await Context.Lancamentos.FindAsync(idLancDes);
        Assert.Null(lancDes);

        var contaDes = await Context.MeiosPagamento.FindAsync(mDes.Id);

        Assert.NotNull(contaDes);
        Assert.Equal(TestConstants.USUARIO_COMUM_ID, contaDes!.IdUsuario);
        Assert.Equal(0, contaDes.Saldo);

        //fatura
        var fatdb = await Context.Faturas.FindAsync(fat.Id);

        Assert.NotNull(fatdb);
        Assert.Equal(40, fatdb!.ValorPago);
    }
}
