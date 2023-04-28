﻿using System.Net.Http.Json;
using K4os.Compression.LZ4.Internal;
using MoneyLoris.Application.Business.Lancamentos.Dtos;
using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Shared;
using MoneyLoris.Tests.Integration.Setup.Utils;
using MoneyLoris.Tests.Integration.Tests.Base;

namespace MoneyLoris.Tests.Integration.Tests.Transferencias;
public class TransferenciaController_InserirTests : IntegrationTestsBase
{
    #region Validacoes

    [Fact]
    public async Task Lancar_UsuarioAdmin_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Administrador);
        await DbSeeder.InserirUsuarios();

        var mOri = await DbSeeder.InserirMeioPagamento(nome: "Conta Ori", saldo: 400);
        var mDes = await DbSeeder.InserirMeioPagamento(nome: "Conta Des", saldo: 200);

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
    public async Task Lancar_MeioPagamentoOrigemNaoEncontrado_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var mDes = await DbSeeder.InserirMeioPagamento(nome: "Conta Des", saldo: 200);

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
    public async Task Lancar_MeioPagamentoOrigemNaoPertenceUsuario_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var mOri = await DbSeeder.InserirMeioPagamento(nome: "Conta Ori", saldo: 400, idUsuario: TestConstants.USUARIO_COMUM_B_ID);
        var mDes = await DbSeeder.InserirMeioPagamento(nome: "Conta Des", saldo: 200);

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
    public async Task Lancar_MeioPagamentoOrigemEhCartaoCredito_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var mOri = await DbSeeder.InserirMeioPagamento(nome: "Conta Ori", saldo: 400, tipo: TipoMeioPagamento.CartaoCredito);
        var mDes = await DbSeeder.InserirMeioPagamento(nome: "Conta Des", saldo: 200);

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
    public async Task Lancar_MeioPagamentoDestinoNaoEncontrado_RetornaErro()
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

        var response = await HttpClient.PostAsJsonAsync("/transferencia/lancar/entreContas", dto);

        //Assert
        var ret = await response.AssertResultNotOk(
            ErrorCodes.MeioPagamento_DestinoNaoEncontrado);
    }

    [Fact]
    public async Task Lancar_MeioPagamentoDestinoNaoPertenceUsuario_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var mOri = await DbSeeder.InserirMeioPagamento(nome: "Conta Ori", saldo: 400);
        var mDes = await DbSeeder.InserirMeioPagamento(nome: "Conta Des", saldo: 200, idUsuario: TestConstants.USUARIO_COMUM_B_ID);

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
    public async Task LancarEntreContas_DestinoEhCartaoCredito_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var mOri = await DbSeeder.InserirMeioPagamento(nome: "Conta Ori", saldo: 400);
        var mDes = await DbSeeder.InserirMeioPagamento(nome: "Conta Des", saldo: 200, tipo: TipoMeioPagamento.CartaoCredito);

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

    #endregion

    [Fact]
    public async Task LancarEntreContas_DadosCertos_TransferenciaRealzada_SaldosAtualizados()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var mOri = await DbSeeder.InserirMeioPagamento(nome: "Conta Ori", saldo: 400);
        var mDes = await DbSeeder.InserirMeioPagamento(nome: "Conta Des", saldo: 200);

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
        Assert.True(laOri.Realizado);
        Assert.True(laOri.IdLancamentoTransferencia > 0);

        //conta origem
        var meOri = await Context.MeiosPagamento.FindAsync(dto.IdMeioPagamentoOrigem);

        Assert.NotNull(meOri);
        Assert.Equal(TestConstants.USUARIO_COMUM_ID, meOri!.IdUsuario);
        Assert.Equal(350, meOri.Saldo);

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
        Assert.True(laDes.Realizado);
        Assert.Equal(laDes.IdLancamentoTransferencia, laOri.Id);

        //conta destino
        var meDes = await Context.MeiosPagamento.FindAsync(dto.IdMeioPagamentoDestino);

        Assert.NotNull(meOri);
        Assert.Equal(TestConstants.USUARIO_COMUM_ID, meDes!.IdUsuario);
        Assert.Equal(250, meDes.Saldo);
    }

    [Fact]
    public async Task LancarPagamentoFatura_DadosCertos_TransferenciaRealzada_SaldoMeioOrigemAtualiza_SaldoMeioDestinoNaoAtualiza()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var mOri = await DbSeeder.InserirMeioPagamento(nome: "Conta Ori", saldo: 700);
        var mDes = await DbSeeder.InserirMeioPagamento(nome: "Conta Des", saldo: 0, tipo: TipoMeioPagamento.CartaoCredito);

        //Act
        var dto = new TransferenciaInsertDto
        {
            Data = SystemTime.Today(),
            IdMeioPagamentoOrigem = mOri.Id,
            IdMeioPagamentoDestino = mDes.Id,
            Valor = 80
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

        //conta destino
        var meDes = await Context.MeiosPagamento.FindAsync(dto.IdMeioPagamentoDestino);

        Assert.NotNull(meOri);
        Assert.Equal(TestConstants.USUARIO_COMUM_ID, meDes!.IdUsuario);
        Assert.Equal(0, meDes.Saldo);
    }
}
