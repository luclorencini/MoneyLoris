using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using MoneyLoris.Application.Business.Lancamentos.Dtos;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Shared;
using MoneyLoris.Tests.Integration.Setup.Utils;
using MoneyLoris.Tests.Integration.Tests.Base;

namespace MoneyLoris.Tests.Integration.Tests.Transferencias;
public class TransferenciaController_LancarPagamentoFaturaTests : IntegrationTestsBase
{
    #region Validacoes

    [Fact]
    public async Task LancarPagamentoFatura_UsuarioAdmin_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Administrador);
        await DbSeeder.InserirUsuarios();

        var mOri = await DbSeeder.InserirMeioPagamento(nome: "Conta Ori", saldo: 400);
        var mDes = await DbSeeder.InserirMeioPagamento(nome: "Cartão Des", saldo: 0, tipo: TipoMeioPagamento.CartaoCredito);

        //Act
        var dto = new TransferenciaInsertDto
        {
            Data = SystemTime.Today(),
            IdMeioPagamentoOrigem = mOri.Id,
            IdMeioPagamentoDestino = mDes.Id,
            Valor = 50
        };

        var response = await HttpClient.PostAsJsonAsync("/transferencia/lancar/pagamentoFatura", dto);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.Transferencia_AdminNaoPode);
    }

    [Fact]
    public async Task LancarPagamentoFatura_MeioPagamentoOrigemNaoEncontrado_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var mDes = await DbSeeder.InserirMeioPagamento(nome: "Cartão Des", saldo: 0, tipo: TipoMeioPagamento.CartaoCredito);

        //Act
        var dto = new TransferenciaInsertDto
        {
            Data = SystemTime.Today(),
            IdMeioPagamentoOrigem = 555,
            IdMeioPagamentoDestino = mDes.Id,
            Valor = 50
        };

        var response = await HttpClient.PostAsJsonAsync("/transferencia/lancar/pagamentoFatura", dto);

        //Assert
        var ret = await response.AssertResultNotOk(
            ErrorCodes.MeioPagamento_OrigemNaoEncontrado);
    }

    [Fact]
    public async Task LancarPagamentoFatura_MeioPagamentoOrigemNaoPertenceUsuario_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var mOri = await DbSeeder.InserirMeioPagamento(nome: "Conta Ori", saldo: 400, idUsuario: TestConstants.USUARIO_COMUM_B_ID);
        var mDes = await DbSeeder.InserirMeioPagamento(nome: "Cartão Des", saldo: 0, tipo: TipoMeioPagamento.CartaoCredito);

        //Act
        var dto = new TransferenciaInsertDto
        {
            Data = SystemTime.Today(),
            IdMeioPagamentoOrigem = mOri.Id,
            IdMeioPagamentoDestino = mDes.Id,
            Valor = 50
        };

        var response = await HttpClient.PostAsJsonAsync("/transferencia/lancar/pagamentoFatura", dto);

        //Assert
        var ret = await response.AssertResultNotOk(
            ErrorCodes.MeioPagamento_OrigemNaoPertenceAoUsuario);
    }

    [Fact]
    public async Task LancarPagamentoFatura_MeioPagamentoOrigemEhCartaoCredito_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var mOri = await DbSeeder.InserirMeioPagamento(nome: "Conta Ori", saldo: 400, tipo: TipoMeioPagamento.CartaoCredito);
        var mDes = await DbSeeder.InserirMeioPagamento(nome: "Cartão Des", saldo: 0, tipo: TipoMeioPagamento.CartaoCredito);

        //Act
        var dto = new TransferenciaInsertDto
        {
            Data = SystemTime.Today(),
            IdMeioPagamentoOrigem = mOri.Id,
            IdMeioPagamentoDestino = mDes.Id,
            Valor = 50
        };

        var response = await HttpClient.PostAsJsonAsync("/transferencia/lancar/pagamentoFatura", dto);

        //Assert
        var ret = await response.AssertResultNotOk(
            ErrorCodes.Transferencia_MeioOrigemNaoPodeSerCartao);
    }

    [Fact]
    public async Task LancarPagamentoFatura_MeioPagamentoDestinoNaoEncontrado_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var mOri = await DbSeeder.InserirMeioPagamento(nome: "Conta Ori", saldo: 200);

        //Act
        var dto = new TransferenciaInsertDto
        {
            Data = SystemTime.Today(),
            IdMeioPagamentoOrigem = mOri.Id,
            IdMeioPagamentoDestino = 555,
            Valor = 50
        };

        var response = await HttpClient.PostAsJsonAsync("/transferencia/lancar/pagamentoFatura", dto);

        //Assert
        var ret = await response.AssertResultNotOk(
            ErrorCodes.MeioPagamento_DestinoNaoEncontrado);
    }

    [Fact]
    public async Task LancarPagamentoFatura_MeioPagamentoDestinoNaoPertenceUsuario_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var mOri = await DbSeeder.InserirMeioPagamento(nome: "Conta Ori", saldo: 400);
        var mDes = await DbSeeder.InserirMeioPagamento(nome: "Cartão Des", saldo: 0, tipo: TipoMeioPagamento.CartaoCredito, idUsuario: TestConstants.USUARIO_COMUM_B_ID);

        //Act
        var dto = new TransferenciaInsertDto
        {
            Data = SystemTime.Today(),
            IdMeioPagamentoOrigem = mOri.Id,
            IdMeioPagamentoDestino = mDes.Id,
            Valor = 50
        };

        var response = await HttpClient.PostAsJsonAsync("/transferencia/lancar/pagamentoFatura", dto);

        //Assert
        var ret = await response.AssertResultNotOk(
            ErrorCodes.MeioPagamento_DestinoNaoPertenceAoUsuario);
    }

    [Fact]
    public async Task LancarPagamentoFatura_DestinoNaoEhCartaoCredito_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var mOri = await DbSeeder.InserirMeioPagamento(nome: "Conta Ori", saldo: 400);
        var mDes = await DbSeeder.InserirMeioPagamento(nome: "Conta Des", saldo: 200, tipo: TipoMeioPagamento.ContaPagamento);

        //Act
        var dto = new TransferenciaInsertDto
        {
            Data = SystemTime.Today(),
            IdMeioPagamentoOrigem = mOri.Id,
            IdMeioPagamentoDestino = mDes.Id,
            Valor = 50
        };

        var response = await HttpClient.PostAsJsonAsync("/transferencia/lancar/pagamentoFatura", dto);

        //Assert
        var ret = await response.AssertResultNotOk(
            ErrorCodes.Transferencia_PagamentoFaturaDestinoTemQueSerCartao);
    }

    [Fact]
    public async Task LancarPagamentoFatura_AnoMesFaturaNaoInformados_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var mOri = await DbSeeder.InserirMeioPagamento(nome: "Conta Ori", saldo: 400);
        var mDes = await DbSeeder.InserirMeioPagamento(nome: "Cartão Des", saldo: 0, tipo: TipoMeioPagamento.CartaoCredito);

        //Act
        var dto = new TransferenciaInsertDto
        {
            Data = SystemTime.Today(),
            IdMeioPagamentoOrigem = mOri.Id,
            IdMeioPagamentoDestino = mDes.Id,
            Valor = 50
        };

        var response = await HttpClient.PostAsJsonAsync("/transferencia/lancar/pagamentoFatura", dto);

        //Assert
        var ret = await response.AssertResultNotOk(
            ErrorCodes.Transferencia_PagamentoFaturaAnoMesFaturaDevemSerInformados);
    }

    #endregion

    [Fact]
    public async Task LancarPagamentoFatura_DadosCertos_FaturaExiste_TransferenciaRealzada_SaldoMeioOrigemAtualiza_SaldoMeioDestinoNaoAtualiza_FaturaAtualizaValorPago()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var mOri = await DbSeeder.InserirMeioPagamento(nome: "Conta Ori", saldo: 700);
        var mDes = await DbSeeder.InserirMeioPagamento(nome: "Cartão Des", saldo: 0, tipo: TipoMeioPagamento.CartaoCredito);
        var fat = await DbSeeder.InserirFatura(mDes.Id, 11, 2023);

        //Act
        var dto = new TransferenciaInsertDto
        {
            Data = new DateTime(2023, 12, 10, 15, 50, 00),
            IdMeioPagamentoOrigem = mOri.Id,
            IdMeioPagamentoDestino = mDes.Id,
            Valor = 80,
            FaturaMes = 11,
            FaturaAno = 2023
        };

        var response = await HttpClient.PostAsJsonAsync("/transferencia/lancar/pagamentoFatura", dto);

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
        Assert.Equal(TipoTransferencia.PagamentoFatura, laOri.TipoTransferencia);
        Assert.Equal(dto.Data, laOri!.Data);
        Assert.Contains("Pagamento Fatura", laOri.Descricao);
        Assert.Equal(-80, laOri.Valor);
        Assert.Null(laOri.IdCategoria);
        Assert.Null(laOri.IdSubcategoria);
        Assert.True(laOri.Realizado);
        Assert.True(laOri.IdLancamentoTransferencia > 0);
        Assert.Equal(laOri.IdFatura, fat.Id);

        //conta origem
        var meOri = await Context.MeiosPagamento.FindAsync(dto.IdMeioPagamentoOrigem);

        Assert.NotNull(meOri);
        Assert.Equal(TestConstants.USUARIO_COMUM_ID, meOri!.IdUsuario);
        Assert.Equal(620, meOri.Saldo);

        //lançamento destino
        var laDes = await Context.Lancamentos.FindAsync(laOri.IdLancamentoTransferencia);

        Assert.NotNull(laDes);
        Assert.Equal(TestConstants.USUARIO_COMUM_ID, laDes!.IdUsuario);
        Assert.Equal(dto.IdMeioPagamentoDestino, laDes.IdMeioPagamento);
        Assert.Equal(TipoLancamento.Receita, laDes.Tipo);
        Assert.Equal(OperacaoLancamento.Transferencia, laDes.Operacao);
        Assert.Equal(TipoTransferencia.PagamentoFatura, laDes.TipoTransferencia);
        Assert.Equal(dto.Data, laDes!.Data);
        Assert.Contains("Pagamento Fatura via", laDes.Descricao);
        Assert.Equal(80, laDes.Valor);
        Assert.Null(laDes.IdCategoria);
        Assert.Null(laDes.IdSubcategoria);
        Assert.True(laDes.Realizado);
        Assert.Equal(laDes.IdLancamentoTransferencia, laOri.Id);
        Assert.Equal(laDes.IdFatura, fat.Id);

        //conta destino
        var meDes = await Context.MeiosPagamento.FindAsync(dto.IdMeioPagamentoDestino);

        Assert.NotNull(meOri);
        Assert.Equal(TestConstants.USUARIO_COMUM_ID, meDes!.IdUsuario);
        Assert.Equal(0, meDes.Saldo);

        //fatura
        var fatdb = await Context.Faturas.FindAsync(fat.Id);

        Assert.NotNull(fatdb);
        Assert.Equal(80, fatdb!.ValorPago);
    }

    [Fact]
    public async Task LancarPagamentoFatura_DadosCertos_FaturaNaoExiste_TransferenciaRealzada_SaldoMeioOrigemAtualiza_SaldoMeioDestinoNaoAtualiza_CriaFaturaComValorPago()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var mOri = await DbSeeder.InserirMeioPagamento(nome: "Conta Ori", saldo: 800);
        var mDes = await DbSeeder.InserirMeioPagamento(nome: "Cartão Des", saldo: 0, tipo: TipoMeioPagamento.CartaoCredito);

        //Act
        var dto = new TransferenciaInsertDto
        {
            Data = new DateTime(2024, 02, 10, 18, 33, 00),
            IdMeioPagamentoOrigem = mOri.Id,
            IdMeioPagamentoDestino = mDes.Id,
            Valor = 140,
            FaturaMes = 1,
            FaturaAno = 2024
        };

        var response = await HttpClient.PostAsJsonAsync("/transferencia/lancar/pagamentoFatura", dto);

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
        Assert.Equal(TipoTransferencia.PagamentoFatura, laOri.TipoTransferencia);
        Assert.Equal(dto.Data, laOri!.Data);
        Assert.Contains("Pagamento Fatura", laOri.Descricao);
        Assert.Equal(-140, laOri.Valor);
        Assert.Null(laOri.IdCategoria);
        Assert.Null(laOri.IdSubcategoria);
        Assert.True(laOri.Realizado);
        Assert.True(laOri.IdLancamentoTransferencia > 0);

        //conta origem
        var meOri = await Context.MeiosPagamento.FindAsync(dto.IdMeioPagamentoOrigem);

        Assert.NotNull(meOri);
        Assert.Equal(TestConstants.USUARIO_COMUM_ID, meOri!.IdUsuario);
        Assert.Equal(660, meOri.Saldo);

        //lançamento destino
        var laDes = await Context.Lancamentos.FindAsync(laOri.IdLancamentoTransferencia);

        Assert.NotNull(laDes);
        Assert.Equal(TestConstants.USUARIO_COMUM_ID, laDes!.IdUsuario);
        Assert.Equal(dto.IdMeioPagamentoDestino, laDes.IdMeioPagamento);
        Assert.Equal(TipoLancamento.Receita, laDes.Tipo);
        Assert.Equal(OperacaoLancamento.Transferencia, laDes.Operacao);
        Assert.Equal(TipoTransferencia.PagamentoFatura, laDes.TipoTransferencia);
        Assert.Equal(dto.Data, laDes!.Data);
        Assert.Contains("Pagamento Fatura via", laDes.Descricao);
        Assert.Equal(140, laDes.Valor);
        Assert.Null(laDes.IdCategoria);
        Assert.Null(laDes.IdSubcategoria);
        Assert.True(laDes.Realizado);
        Assert.Equal(laDes.IdLancamentoTransferencia, laOri.Id);

        //conta destino
        var meDes = await Context.MeiosPagamento.FindAsync(dto.IdMeioPagamentoDestino);

        Assert.NotNull(meOri);
        Assert.Equal(TestConstants.USUARIO_COMUM_ID, meDes!.IdUsuario);
        Assert.Equal(0, meDes.Saldo);

        //fatura
        var fatdb = await Context.Faturas.FirstOrDefaultAsync();

        Assert.NotNull(fatdb);
        Assert.Equal(140, fatdb!.ValorPago);
        Assert.Equal(laDes.IdFatura, fatdb.Id);
        Assert.Equal(laOri.IdFatura, fatdb.Id);
    }
}
