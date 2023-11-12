using System.Net.Http.Json;
using MoneyLoris.Application.Business.Lancamentos.Dtos;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Shared;
using MoneyLoris.Tests.Integration.Setup.Utils;
using MoneyLoris.Tests.Integration.Tests.Base;

namespace MoneyLoris.Tests.Integration.Tests.Transferencias;
public class TransferenciaController_LancarTransferenciaEntreContasTests : IntegrationTestsBase
{
    #region Validacoes

    [Fact]
    public async Task LancarEntreContas_UsuarioAdmin_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Administrador);
        await DbSeeder.InserirUsuarios();

        var mOri = await DbSeeder.InserirMeioPagamento(nome: "Conta Ori");
        var mDes = await DbSeeder.InserirMeioPagamento(nome: "Conta Des");

        //Act
        var dto = new TransferenciaInsertDto
        {
            Data = SystemTime.Today(),
            IdMeioPagamentoOrigem = mOri.Id,
            IdMeioPagamentoDestino = mDes.Id,
            Valor = 50
        };

        var response = await HttpClient.PostAsJsonAsync("/transferencia/lancar/entreContas", dto);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.Transferencia_AdminNaoPode);
    }

    [Fact]
    public async Task LancarEntreContas_MeioPagamentoOrigemNaoEncontrado_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var mDes = await DbSeeder.InserirMeioPagamento(nome: "Conta Des");

        //Act
        var dto = new TransferenciaInsertDto
        {
            Data = SystemTime.Today(),
            IdMeioPagamentoOrigem = 555,
            IdMeioPagamentoDestino = mDes.Id,
            Valor = 50
        };

        var response = await HttpClient.PostAsJsonAsync("/transferencia/lancar/entreContas", dto);

        //Assert
        var ret = await response.AssertResultNotOk(
            ErrorCodes.MeioPagamento_OrigemNaoEncontrado);
    }

    [Fact]
    public async Task LancarEntreContas_MeioPagamentoOrigemNaoPertenceUsuario_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var mOri = await DbSeeder.InserirMeioPagamento(nome: "Conta Ori", idUsuario: TestConstants.USUARIO_COMUM_B_ID);
        var mDes = await DbSeeder.InserirMeioPagamento(nome: "Conta Des");

        //Act
        var dto = new TransferenciaInsertDto
        {
            Data = SystemTime.Today(),
            IdMeioPagamentoOrigem = mOri.Id,
            IdMeioPagamentoDestino = mDes.Id,
            Valor = 50
        };

        var response = await HttpClient.PostAsJsonAsync("/transferencia/lancar/entreContas", dto);

        //Assert
        var ret = await response.AssertResultNotOk(
            ErrorCodes.MeioPagamento_OrigemNaoPertenceAoUsuario);
    }

    [Fact]
    public async Task LancarEntreContas_MeioPagamentoOrigemEhCartaoCredito_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var mOri = await DbSeeder.InserirMeioPagamento(nome: "Conta Ori", tipo: TipoMeioPagamento.CartaoCredito);
        var mDes = await DbSeeder.InserirMeioPagamento(nome: "Conta Des");

        //Act
        var dto = new TransferenciaInsertDto
        {
            Data = SystemTime.Today(),
            IdMeioPagamentoOrigem = mOri.Id,
            IdMeioPagamentoDestino = mDes.Id,
            Valor = 50
        };

        var response = await HttpClient.PostAsJsonAsync("/transferencia/lancar/entreContas", dto);

        //Assert
        var ret = await response.AssertResultNotOk(
            ErrorCodes.Transferencia_MeioOrigemNaoPodeSerCartao);
    }

    [Fact]
    public async Task LancarEntreContas_MeioPagamentoDestinoNaoEncontrado_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var mOri = await DbSeeder.InserirMeioPagamento(nome: "Conta Ori");

        //Act
        var dto = new TransferenciaInsertDto
        {
            Data = SystemTime.Today(),
            IdMeioPagamentoOrigem = mOri.Id,
            IdMeioPagamentoDestino = 555,
            Valor = 50
        };

        var response = await HttpClient.PostAsJsonAsync("/transferencia/lancar/entreContas", dto);

        //Assert
        var ret = await response.AssertResultNotOk(
            ErrorCodes.MeioPagamento_DestinoNaoEncontrado);
    }

    [Fact]
    public async Task LancarEntreContas_MeioPagamentoDestinoNaoPertenceUsuario_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var mOri = await DbSeeder.InserirMeioPagamento(nome: "Conta Ori");
        var mDes = await DbSeeder.InserirMeioPagamento(nome: "Conta Des", idUsuario: TestConstants.USUARIO_COMUM_B_ID);

        //Act
        var dto = new TransferenciaInsertDto
        {
            Data = SystemTime.Today(),
            IdMeioPagamentoOrigem = mOri.Id,
            IdMeioPagamentoDestino = mDes.Id,
            Valor = 50
        };

        var response = await HttpClient.PostAsJsonAsync("/transferencia/lancar/entreContas", dto);

        //Assert
        var ret = await response.AssertResultNotOk(
            ErrorCodes.MeioPagamento_DestinoNaoPertenceAoUsuario);
    }

    [Fact]
    public async Task LancarEntreContas_DestinoEhCartaoCredito_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var mOri = await DbSeeder.InserirMeioPagamento(nome: "Conta Ori");
        var mDes = await DbSeeder.InserirMeioPagamento(nome: "Conta Des", tipo: TipoMeioPagamento.CartaoCredito);

        //Act
        var dto = new TransferenciaInsertDto
        {
            Data = SystemTime.Today(),
            IdMeioPagamentoOrigem = mOri.Id,
            IdMeioPagamentoDestino = mDes.Id,
            Valor = 50
        };

        var response = await HttpClient.PostAsJsonAsync("/transferencia/lancar/entreContas", dto);

        //Assert
        var ret = await response.AssertResultNotOk(
            ErrorCodes.Transferencia_EntreContasDestinoNaoPodeSerCartao);
    }

    #endregion

    [Fact]
    public async Task LancarEntreContas_DadosCertos_TransferenciaRealzada()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var mOri = await DbSeeder.InserirMeioPagamento(nome: "Conta Ori");
        var mDes = await DbSeeder.InserirMeioPagamento(nome: "Conta Des");

        //Act
        var dto = new TransferenciaInsertDto
        {
            Data = SystemTime.Today(),
            IdMeioPagamentoOrigem = mOri.Id,
            IdMeioPagamentoDestino = mDes.Id,
            Valor = 50
        };

        var response = await HttpClient.PostAsJsonAsync("/transferencia/lancar/entreContas", dto);

        //Assert
        var idLancamentoOrigem = await response.ConverteResultOk<int>();

        Assert.True(idLancamentoOrigem > 0);

        //lançamento origem
        var laOri = await Context.Lancamentos.FindAsync(idLancamentoOrigem);

        Assert.NotNull(laOri);
        Assert.Equal(TestConstants.USUARIO_COMUM_ID, laOri!.IdUsuario);
        Assert.Equal(dto.IdMeioPagamentoOrigem, laOri.IdMeioPagamento);
        Assert.Equal(TipoLancamento.Despesa, laOri.Tipo);
        Assert.Equal(OperacaoLancamento.Transferencia, laOri.Operacao);
        Assert.Equal(TipoTransferencia.TransferenciaEntreContas, laOri.TipoTransferencia);
        Assert.Equal(dto.Data, laOri!.Data);
        Assert.Contains("Transferência para", laOri.Descricao);
        Assert.Equal(-50, laOri.Valor);
        Assert.Null(laOri.IdCategoria);
        Assert.Null(laOri.IdSubcategoria);
        Assert.Null(laOri.IdFatura);
        Assert.True(laOri.Realizado);
        Assert.True(laOri.IdLancamentoTransferencia > 0);

        //conta origem
        var meOri = await Context.MeiosPagamento.FindAsync(dto.IdMeioPagamentoOrigem);

        Assert.NotNull(meOri);
        Assert.Equal(TestConstants.USUARIO_COMUM_ID, meOri!.IdUsuario);

        //lançamento destino
        var laDes = await Context.Lancamentos.FindAsync(laOri.IdLancamentoTransferencia);

        Assert.NotNull(laDes);
        Assert.Equal(TestConstants.USUARIO_COMUM_ID, laDes!.IdUsuario);
        Assert.Equal(dto.IdMeioPagamentoDestino, laDes.IdMeioPagamento);
        Assert.Equal(TipoLancamento.Receita, laDes.Tipo);
        Assert.Equal(OperacaoLancamento.Transferencia, laDes.Operacao);
        Assert.Equal(TipoTransferencia.TransferenciaEntreContas, laDes.TipoTransferencia);
        Assert.Equal(dto.Data, laDes!.Data);
        Assert.Contains("Transferência de", laDes.Descricao);
        Assert.Equal(50, laDes.Valor);
        Assert.Null(laDes.IdCategoria);
        Assert.Null(laDes.IdSubcategoria);
        Assert.Null(laDes.IdFatura);
        Assert.True(laDes.Realizado);
        Assert.Equal(laDes.IdLancamentoTransferencia, laOri.Id);

        //conta destino
        var meDes = await Context.MeiosPagamento.FindAsync(dto.IdMeioPagamentoDestino);

        Assert.NotNull(meOri);
        Assert.Equal(TestConstants.USUARIO_COMUM_ID, meDes!.IdUsuario);
    }
}
