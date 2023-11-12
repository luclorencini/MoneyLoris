using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
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
            IdCategoria = cat.Id,
            IdMeioPagamento = mei.Id,
            Valor = 10
        };

        var response = await HttpClient.PostAsJsonAsync("/lancamento/lancar/despesa", dto);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.Lancamento_TipoDiferenteDaCategoria);
    }

    [Fact]
    public async Task LancarDespesa_CartaoCredito_SemParcela_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var cat = await DbSeeder.InserirCategoria(TipoLancamento.Despesa);
        var mei = await DbSeeder.InserirMeioPagamento(tipo: TipoMeioPagamento.CartaoCredito);

        //Act
        var dto = new LancamentoCadastroDto
        {
            Data = SystemTime.Today(),
            Descricao = "Compras",
            IdCategoria = cat.Id,
            IdMeioPagamento = mei.Id,
            Valor = 10
        };

        var response = await HttpClient.PostAsJsonAsync("/lancamento/lancar/despesa", dto);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.Lancamento_CartaoCreditoSemParcela);
    }

    #endregion

    [Fact]
    public async Task LancarReceita_DadosCertos_MeioNaoEhCartao_LancamentoRealizado()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var cat = await DbSeeder.InserirCategoria(TipoLancamento.Receita);
        var mei = await DbSeeder.InserirMeioPagamento(tipo: TipoMeioPagamento.ContaCorrente);

        //Act
        var dto = new LancamentoCadastroDto
        {
            Data = SystemTime.Today(),
            Descricao = "Compras",
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
        Assert.Equal(TipoLancamento.Receita, lanc.Tipo);
        Assert.Equal(OperacaoLancamento.LancamentoSimples, lanc.Operacao);
        Assert.Null(lanc.TipoTransferencia);
        Assert.Equal(dto.Data, lanc!.Data);
        Assert.Equal(dto.Descricao, lanc.Descricao);
        Assert.Equal(10, lanc.Valor);
        Assert.Equal(dto.IdCategoria, lanc.IdCategoria);
        Assert.Null(lanc.IdSubcategoria);
        Assert.True(lanc.Realizado);
        Assert.Null(lanc.IdLancamentoTransferencia);
        Assert.Null(lanc.ParcelaAtual);
        Assert.Null(lanc.ParcelaTotal);

        var conta = await Context.MeiosPagamento.FindAsync(mei.Id);

        Assert.NotNull(conta);
        Assert.Equal(TestConstants.USUARIO_COMUM_ID, conta!.IdUsuario);
    }

    [Fact]
    public async Task LancarDespesa_DadosCertos_MeioEhCartao_UmaParcela_SemFatura_LancamentoRealizado_FaturaCriada()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var cat = await DbSeeder.InserirCategoria(TipoLancamento.Despesa);
        var sub = await DbSeeder.InserirSubcategoria(cat.Id);
        var mei = await DbSeeder.InserirMeioPagamento(tipo: TipoMeioPagamento.CartaoCredito, fecha: 5, vence: 15);

        //Act
        var dto = new LancamentoCadastroDto
        {
            Data = SystemTime.Today(),
            Descricao = "Compras",
            IdCategoria = cat.Id,
            IdSubcategoria = sub.Id,
            IdMeioPagamento = mei.Id,
            Valor = 10,
            Parcelas = 1,
            FaturaMes = 5,
            FaturaAno = 2023
        };

        var response = await HttpClient.PostAsJsonAsync("/lancamento/lancar/despesa", dto);

        //Assert
        var idLancamento = await response.ConverteResultOk<int>();

        Assert.True(idLancamento > 0);

        var lanc = await Context.Lancamentos.FindAsync(idLancamento);

        Assert.NotNull(lanc);
        Assert.Equal(TestConstants.USUARIO_COMUM_ID, lanc!.IdUsuario);
        Assert.Equal(dto.IdMeioPagamento, lanc.IdMeioPagamento);
        Assert.Equal(TipoLancamento.Despesa, lanc.Tipo);
        Assert.Equal(OperacaoLancamento.LancamentoSimples, lanc.Operacao);
        Assert.Null(lanc.TipoTransferencia);
        Assert.Equal(dto.Data, lanc!.Data);
        Assert.Equal(dto.Descricao, lanc.Descricao);
        Assert.Equal(-10, lanc.Valor);
        Assert.Equal(dto.IdCategoria, lanc.IdCategoria);
        Assert.Equal(dto.IdSubcategoria, lanc.IdSubcategoria);
        Assert.True(lanc.Realizado);
        Assert.Null(lanc.IdLancamentoTransferencia);
        Assert.Null(lanc.ParcelaAtual);
        Assert.Null(lanc.ParcelaTotal);

        var conta = await Context.MeiosPagamento.FindAsync(mei.Id);

        Assert.NotNull(conta);
        Assert.Equal(TestConstants.USUARIO_COMUM_ID, conta!.IdUsuario);

        var faturas = await Context.Faturas.ToListAsync();
        Assert.NotNull(faturas);
        Assert.True(faturas.Count == 1);

        var fatura = faturas.First();
        Assert.Equal(lanc.IdFatura, fatura.Id);
        Assert.Equal(mei.Id, fatura.IdMeioPagamento);
        Assert.Equal(dto.FaturaMes, fatura.Mes);
        Assert.Equal(dto.FaturaAno, fatura.Ano);
    }

    [Fact]
    public async Task LancarDespesa_DadosCertos_MeioEhCartao_UmaParcela_FaturaJaExiste_LancamentoRealizado()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var cat = await DbSeeder.InserirCategoria(TipoLancamento.Despesa);
        var sub = await DbSeeder.InserirSubcategoria(cat.Id);
        var mei = await DbSeeder.InserirMeioPagamento(tipo: TipoMeioPagamento.CartaoCredito);
        var fat = await DbSeeder.InserirFatura(mei.Id, mes: 6, ano: 2023);

        //Act
        var dto = new LancamentoCadastroDto
        {
            Data = SystemTime.Today(),
            Descricao = "Compras",
            IdCategoria = cat.Id,
            IdSubcategoria = sub.Id,
            IdMeioPagamento = mei.Id,
            Valor = 25,
            Parcelas = 1,
            FaturaMes = 6,
            FaturaAno = 2023
        };

        var response = await HttpClient.PostAsJsonAsync("/lancamento/lancar/despesa", dto);

        //Assert
        var idLancamento = await response.ConverteResultOk<int>();

        Assert.True(idLancamento > 0);

        var lanc = await Context.Lancamentos.FindAsync(idLancamento);

        Assert.NotNull(lanc);
        Assert.Equal(TestConstants.USUARIO_COMUM_ID, lanc!.IdUsuario);
        Assert.Equal(dto.IdMeioPagamento, lanc.IdMeioPagamento);
        Assert.Equal(TipoLancamento.Despesa, lanc.Tipo);
        Assert.Equal(OperacaoLancamento.LancamentoSimples, lanc.Operacao);
        Assert.Null(lanc.TipoTransferencia);
        Assert.Equal(dto.Data, lanc!.Data);
        Assert.Equal(dto.Descricao, lanc.Descricao);
        Assert.Equal(-25, lanc.Valor);
        Assert.Equal(dto.IdCategoria, lanc.IdCategoria);
        Assert.Equal(dto.IdSubcategoria, lanc.IdSubcategoria);
        Assert.True(lanc.Realizado);
        Assert.Null(lanc.IdLancamentoTransferencia);
        Assert.Null(lanc.ParcelaAtual);
        Assert.Null(lanc.ParcelaTotal);

        var conta = await Context.MeiosPagamento.FindAsync(mei.Id);

        Assert.NotNull(conta);
        Assert.Equal(TestConstants.USUARIO_COMUM_ID, conta!.IdUsuario);

        var faturas = await Context.Faturas.ToListAsync();
        Assert.NotNull(faturas);
        Assert.True(faturas.Count == 1);

        var fatura = faturas.First();
        Assert.Equal(mei.Id, fatura.IdMeioPagamento);
        Assert.Equal(fat.Mes, fatura.Mes);
        Assert.Equal(fat.Ano, fatura.Ano);
        Assert.Equal(fat.Id, fatura.Id);
        Assert.Equal(lanc.IdFatura, fatura.Id);
    }

    [Fact]
    public async Task LancarDespesa_DadosCertos_MeioEhCartao_DuasParcelas_SoPrimeiraFaturaExiste_LancamentosRealizados_UmaFaturaCriada()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var cat = await DbSeeder.InserirCategoria(TipoLancamento.Despesa);
        var sub = await DbSeeder.InserirSubcategoria(cat.Id);
        var mei = await DbSeeder.InserirMeioPagamento(tipo: TipoMeioPagamento.CartaoCredito);
        var fat = await DbSeeder.InserirFatura(mei.Id, mes: 8, ano: 2023);

        var dataLanc1 = new DateTime(2023, 4, 28);
        var dataLanc2 = new DateTime(2023, 5, 28);

        //Act
        var dto = new LancamentoCadastroDto
        {
            Data = dataLanc1,
            Descricao = "Compras parceladas",
            IdCategoria = cat.Id,
            IdSubcategoria = sub.Id,
            IdMeioPagamento = mei.Id,
            Valor = 30,
            Parcelas = 2,
            FaturaMes = 8,
            FaturaAno = 2023
        };

        var response = await HttpClient.PostAsJsonAsync("/lancamento/lancar/despesa", dto);

        //Assert
        var idLancamento = await response.ConverteResultOk<int>();

        Assert.True(idLancamento > 0);

        var lancs = await Context.Lancamentos.OrderBy(x => x.Data).ToListAsync();

        Assert.NotNull(lancs);
        Assert.True(lancs.Count == 2);

        var lanc1 = lancs.First();

        Assert.NotNull(lanc1);
        Assert.Equal(idLancamento, lanc1.Id);
        Assert.Equal(TestConstants.USUARIO_COMUM_ID, lanc1!.IdUsuario);
        Assert.Equal(dto.IdMeioPagamento, lanc1.IdMeioPagamento);
        Assert.Equal(TipoLancamento.Despesa, lanc1.Tipo);
        Assert.Equal(OperacaoLancamento.LancamentoSimples, lanc1.Operacao);
        Assert.Null(lanc1.TipoTransferencia);
        Assert.Equal(dataLanc1, lanc1!.Data);
        Assert.Equal("Compras parceladas", lanc1.Descricao);
        Assert.Equal(-15, lanc1.Valor);
        Assert.Equal(dto.IdCategoria, lanc1.IdCategoria);
        Assert.Equal(dto.IdSubcategoria, lanc1.IdSubcategoria);
        Assert.True(lanc1.Realizado);
        Assert.Null(lanc1.IdLancamentoTransferencia);
        Assert.Equal((short)1, lanc1.ParcelaAtual);
        Assert.Equal((short)2, lanc1.ParcelaTotal);


        var lanc2 = lancs.Last();

        Assert.NotNull(lanc2);
        Assert.Equal(TestConstants.USUARIO_COMUM_ID, lanc2!.IdUsuario);
        Assert.Equal(dto.IdMeioPagamento, lanc2.IdMeioPagamento);
        Assert.Equal(TipoLancamento.Despesa, lanc2.Tipo);
        Assert.Equal(OperacaoLancamento.LancamentoSimples, lanc2.Operacao);
        Assert.Null(lanc2.TipoTransferencia);
        Assert.Equal(dataLanc2, lanc2!.Data);
        Assert.Equal("Compras parceladas", lanc2.Descricao);
        Assert.Equal(-15, lanc2.Valor);
        Assert.Equal(dto.IdCategoria, lanc2.IdCategoria);
        Assert.Equal(dto.IdSubcategoria, lanc2.IdSubcategoria);
        Assert.True(lanc2.Realizado);
        Assert.Null(lanc2.IdLancamentoTransferencia);
        Assert.Equal((short)2, lanc2.ParcelaAtual);
        Assert.Equal((short)2, lanc2.ParcelaTotal);


        var conta = await Context.MeiosPagamento.FindAsync(mei.Id);

        Assert.NotNull(conta);
        Assert.Equal(TestConstants.USUARIO_COMUM_ID, conta!.IdUsuario);

        var faturas = await Context.Faturas.ToListAsync();
        Assert.NotNull(faturas);
        Assert.True(faturas.Count == 2);

        var fat1 = faturas.First();
        Assert.Equal(mei.Id, fat1.IdMeioPagamento);
        Assert.Equal(8, fat1.Mes);
        Assert.Equal(2023, fat1.Ano);
        Assert.Equal(fat.Id, fat1.Id);
        Assert.Equal(lanc1.IdFatura, fat1.Id);

        var fat2 = faturas.Last();
        Assert.Equal(lanc2.IdFatura, fat2.Id);
        Assert.Equal(mei.Id, fat2.IdMeioPagamento);
        Assert.Equal(9, fat2.Mes);
        Assert.Equal(2023, fat2.Ano);
    }
}
