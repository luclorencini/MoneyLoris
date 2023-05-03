using System.Net.Http.Json;
using MoneyLoris.Application.Business.Lancamentos.Dtos;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Shared;
using MoneyLoris.Tests.Integration.Setup.Utils;
using MoneyLoris.Tests.Integration.Tests.Base;

namespace MoneyLoris.Tests.Integration.Tests.Lancamentos;
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

        var cat = await DbSeeder.InserirCategoria();
        var sub = await DbSeeder.InserirSubcategoria(cat.Id);
        var mei = await DbSeeder.InserirMeioPagamento();
        var lanc = await DbSeeder.InserirLancamentoSimples(mei.Id, cat.Id, sub.Id);


        //Act
        var dto = new LancamentoCadastroDto
        {
            Id = lanc.Id,
            Data = SystemTime.Today(),
            Descricao = "Compras",
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

        var cat = await DbSeeder.InserirCategoria();
        var sub = await DbSeeder.InserirSubcategoria(cat.Id);
        var mei = await DbSeeder.InserirMeioPagamento();
        var lanc = await DbSeeder.InserirLancamentoSimples(mei.Id, cat.Id, sub.Id);

        var catB = await DbSeeder.InserirCategoria(nome: "Diversos");
        var subB = await DbSeeder.InserirSubcategoria(catB.Id);

        //Act
        var dto = new LancamentoCadastroDto
        {
            Id = lanc.Id,
            Data = SystemTime.Today(),
            Descricao = "Compras",
            IdCategoria = cat.Id,
            IdSubcategoria = subB.Id,
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
    public async Task Alterar_Despesa_Conta_ValorNaoMudou_DadosAlterados_SaldoSeMantem()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var cat = await DbSeeder.InserirCategoria(tipo: TipoLancamento.Despesa);
        var sub = await DbSeeder.InserirSubcategoria(cat.Id);
        var mei = await DbSeeder.InserirMeioPagamento(saldo: 200, tipo: TipoMeioPagamento.ContaCorrente);
        var lanc = await DbSeeder.InserirLancamentoSimples(mei.Id, cat.Id, sub.Id, valor: -50, tipo: TipoLancamento.Despesa);

        var catB = await DbSeeder.InserirCategoria(nome: "Diversos", tipo: TipoLancamento.Despesa);
        var subB = await DbSeeder.InserirSubcategoria(catB.Id);

        //Act
        var dto = new LancamentoCadastroDto
        {
            Id = lanc.Id,
            Data = SystemTime.Today().AddDays(1),
            Descricao = "Gastos",
            IdCategoria = catB.Id,
            IdSubcategoria = subB.Id,
            IdMeioPagamento = mei.Id,
            Valor = 50
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
        Assert.True(ldb.Valor < 0);  //lançamento de Despesa é sempre negativo
        Assert.Equal(-50, ldb.Valor);

        var conta = await Context.MeiosPagamento.FindAsync(mei.Id);

        Assert.NotNull(conta);
        Assert.Equal(TestConstants.USUARIO_COMUM_ID, conta!.IdUsuario);
        Assert.Equal(200, conta.Saldo);  //se o valor nao muda, o saldo tambem nao
    }

    [Fact]
    public async Task Alterar_Despesa_Conta_ValorMudouPraMenos_DadosAlterados_SaldoAumenta()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var cat = await DbSeeder.InserirCategoria(tipo: TipoLancamento.Despesa);
        var sub = await DbSeeder.InserirSubcategoria(cat.Id);
        var mei = await DbSeeder.InserirMeioPagamento(saldo: 400, tipo: TipoMeioPagamento.Poupanca);
        var lanc = await DbSeeder.InserirLancamentoSimples(mei.Id, cat.Id, sub.Id, valor: -90, tipo: TipoLancamento.Despesa);

        var catB = await DbSeeder.InserirCategoria(nome: "Diversos", tipo: TipoLancamento.Despesa);
        var subB = await DbSeeder.InserirSubcategoria(catB.Id);

        //Act
        var dto = new LancamentoCadastroDto
        {
            Id = lanc.Id,
            Data = SystemTime.Today().AddDays(1),
            Descricao = "Gastos",
            IdCategoria = catB.Id,
            IdSubcategoria = subB.Id,
            IdMeioPagamento = mei.Id,
            Valor = 70
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
        Assert.True(ldb.Valor < 0);  //lançamento de Despesa é sempre negativo
        Assert.Equal(-70, ldb.Valor);  //valor mudou

        var conta = await Context.MeiosPagamento.FindAsync(mei.Id);

        Assert.NotNull(conta);
        Assert.Equal(TestConstants.USUARIO_COMUM_ID, conta!.IdUsuario);
        Assert.Equal(420, conta.Saldo);  //se o valor muda, o saldo atualiza
    }

    [Fact]
    public async Task Alterar_Despesa_Conta_ValorMudouPraMais_DadosAlterados_SaldoDiminui()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var cat = await DbSeeder.InserirCategoria(tipo: TipoLancamento.Despesa);
        var sub = await DbSeeder.InserirSubcategoria(cat.Id);
        var mei = await DbSeeder.InserirMeioPagamento(saldo: 400, tipo: TipoMeioPagamento.CartaoBeneficio);
        var lanc = await DbSeeder.InserirLancamentoSimples(mei.Id, cat.Id, sub.Id, valor: -90, tipo: TipoLancamento.Despesa);

        var catB = await DbSeeder.InserirCategoria(nome: "Diversos", tipo: TipoLancamento.Despesa);
        var subB = await DbSeeder.InserirSubcategoria(catB.Id);

        //Act
        var dto = new LancamentoCadastroDto
        {
            Id = lanc.Id,
            Data = SystemTime.Today().AddDays(1),
            Descricao = "Gastos",
            IdCategoria = catB.Id,
            IdSubcategoria = subB.Id,
            IdMeioPagamento = mei.Id,
            Valor = 180
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
        Assert.True(ldb.Valor < 0);  //lançamento de Despesa é sempre negativo
        Assert.Equal(-180, ldb.Valor);  //valor mudou

        var conta = await Context.MeiosPagamento.FindAsync(mei.Id);

        Assert.NotNull(conta);
        Assert.Equal(TestConstants.USUARIO_COMUM_ID, conta!.IdUsuario);
        Assert.Equal(310, conta.Saldo);  //se o valor muda, o saldo atualiza
    }

    [Fact]
    public async Task Alterar_Despesa_CartaoCredito_ValorMudou_DadosAlterados_SaldoDeCartaoEhSempreZero()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var cat = await DbSeeder.InserirCategoria(tipo: TipoLancamento.Despesa);
        var sub = await DbSeeder.InserirSubcategoria(cat.Id);
        var mei = await DbSeeder.InserirMeioPagamento(saldo: 0, tipo: TipoMeioPagamento.CartaoCredito);
        var lanc = await DbSeeder.InserirLancamentoSimples(mei.Id, cat.Id, sub.Id, valor: -100, tipo: TipoLancamento.Despesa);

        var catB = await DbSeeder.InserirCategoria(nome: "Diversos", tipo: TipoLancamento.Despesa);
        var subB = await DbSeeder.InserirSubcategoria(catB.Id);

        //Act
        var dto = new LancamentoCadastroDto
        {
            Id = lanc.Id,
            Data = SystemTime.Today().AddDays(1),
            Descricao = "Gastos",
            IdCategoria = catB.Id,
            IdSubcategoria = subB.Id,
            IdMeioPagamento = mei.Id,
            Valor = 60
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
        Assert.True(ldb.Valor < 0);  //lançamento de Despesa é sempre negativo
        Assert.Equal(-60, ldb.Valor);  //valor mudou

        var conta = await Context.MeiosPagamento.FindAsync(mei.Id);

        Assert.NotNull(conta);
        Assert.Equal(TestConstants.USUARIO_COMUM_ID, conta!.IdUsuario);
        Assert.Equal(0, conta.Saldo);  //saldo de cartão de crédito é sempre zero
    }

    [Fact]
    public async Task Alterar_Receita_Conta_ValorNaoMudou_DadosAlterados_SaldoSeMantem()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var cat = await DbSeeder.InserirCategoria(tipo: TipoLancamento.Receita);
        var sub = await DbSeeder.InserirSubcategoria(cat.Id);
        var mei = await DbSeeder.InserirMeioPagamento(saldo: 300, tipo: TipoMeioPagamento.Carteira);
        var lanc = await DbSeeder.InserirLancamentoSimples(mei.Id, cat.Id, sub.Id, valor: 40, tipo: TipoLancamento.Receita);

        var catB = await DbSeeder.InserirCategoria(nome: "Diversos", tipo: TipoLancamento.Receita);
        var subB = await DbSeeder.InserirSubcategoria(catB.Id);

        //Act
        var dto = new LancamentoCadastroDto
        {
            Id = lanc.Id,
            Data = SystemTime.Today().AddDays(1),
            Descricao = "Rendimentos",
            IdCategoria = catB.Id,
            IdSubcategoria = subB.Id,
            IdMeioPagamento = mei.Id,
            Valor = 40
        };

        var response = await HttpClient.PostAsJsonAsync("/lancamento/alterar", dto);

        //Assert
        var idLancamento = await response.ConverteResultOk<int>();

        Assert.True(idLancamento > 0);

        var ldb = await Context.Lancamentos.FindAsync(idLancamento);

        Assert.NotNull(lanc);
        Assert.Equal(TestConstants.USUARIO_COMUM_ID, ldb!.IdUsuario);
        Assert.Equal(dto.IdMeioPagamento, ldb.IdMeioPagamento);
        Assert.Equal(TipoLancamento.Receita, ldb.Tipo);
        Assert.Equal(OperacaoLancamento.LancamentoSimples, ldb.Operacao);
        Assert.Null(lanc.TipoTransferencia);
        Assert.Equal(dto.Data, ldb!.Data);
        Assert.Equal(dto.Descricao, ldb.Descricao);
        Assert.Equal(dto.IdCategoria, ldb.IdCategoria);
        Assert.Equal(dto.IdSubcategoria, ldb.IdSubcategoria);
        Assert.True(ldb.Realizado);
        Assert.Null(ldb.IdLancamentoTransferencia);
        Assert.True(ldb.Valor > 0);  //lançamento de Receita é sempre positivo
        Assert.Equal(40, ldb.Valor);

        var conta = await Context.MeiosPagamento.FindAsync(mei.Id);

        Assert.NotNull(conta);
        Assert.Equal(TestConstants.USUARIO_COMUM_ID, conta!.IdUsuario);
        Assert.Equal(300, conta.Saldo);  //se o valor nao muda, o saldo tambem nao
    }

    [Fact]
    public async Task Alterar_Receita_Conta_ValorMudouPraMais_DadosAlterados_SaldoAumenta()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var cat = await DbSeeder.InserirCategoria(tipo: TipoLancamento.Receita);
        var sub = await DbSeeder.InserirSubcategoria(cat.Id);
        var mei = await DbSeeder.InserirMeioPagamento(saldo: 500, tipo: TipoMeioPagamento.ContaPagamento);
        var lanc = await DbSeeder.InserirLancamentoSimples(mei.Id, cat.Id, sub.Id, valor: 110, tipo: TipoLancamento.Receita);

        var catB = await DbSeeder.InserirCategoria(nome: "Diversos", tipo: TipoLancamento.Receita);
        var subB = await DbSeeder.InserirSubcategoria(catB.Id);

        //Act
        var dto = new LancamentoCadastroDto
        {
            Id = lanc.Id,
            Data = SystemTime.Today().AddDays(1),
            Descricao = "Rendimentos",
            IdCategoria = catB.Id,
            IdSubcategoria = subB.Id,
            IdMeioPagamento = mei.Id,
            Valor = 150
        };

        var response = await HttpClient.PostAsJsonAsync("/lancamento/alterar", dto);

        //Assert
        var idLancamento = await response.ConverteResultOk<int>();

        Assert.True(idLancamento > 0);

        var ldb = await Context.Lancamentos.FindAsync(idLancamento);

        Assert.NotNull(lanc);
        Assert.Equal(TestConstants.USUARIO_COMUM_ID, ldb!.IdUsuario);
        Assert.Equal(dto.IdMeioPagamento, ldb.IdMeioPagamento);
        Assert.Equal(TipoLancamento.Receita, ldb.Tipo);
        Assert.Equal(OperacaoLancamento.LancamentoSimples, ldb.Operacao);
        Assert.Null(lanc.TipoTransferencia);
        Assert.Equal(dto.Data, ldb!.Data);
        Assert.Equal(dto.Descricao, ldb.Descricao);
        Assert.Equal(dto.IdCategoria, ldb.IdCategoria);
        Assert.Equal(dto.IdSubcategoria, ldb.IdSubcategoria);
        Assert.True(ldb.Realizado);
        Assert.Null(ldb.IdLancamentoTransferencia);
        Assert.True(ldb.Valor > 0);  //lançamento de Receita é sempre positivo
        Assert.Equal(150, ldb.Valor);

        var conta = await Context.MeiosPagamento.FindAsync(mei.Id);

        Assert.NotNull(conta);
        Assert.Equal(TestConstants.USUARIO_COMUM_ID, conta!.IdUsuario);
        Assert.Equal(540, conta.Saldo);  //se o valor muda, o saldo atualiza
    }

    [Fact]
    public async Task Alterar_Receita_Conta_ValorMudouPraMenos_DadosAlterados_SaldoDiminui()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var cat = await DbSeeder.InserirCategoria(tipo: TipoLancamento.Receita);
        var sub = await DbSeeder.InserirSubcategoria(cat.Id);
        var mei = await DbSeeder.InserirMeioPagamento(saldo: 500, tipo: TipoMeioPagamento.ContaPagamento);
        var lanc = await DbSeeder.InserirLancamentoSimples(mei.Id, cat.Id, sub.Id, valor: 110, tipo: TipoLancamento.Receita);

        var catB = await DbSeeder.InserirCategoria(nome: "Diversos", tipo: TipoLancamento.Receita);
        var subB = await DbSeeder.InserirSubcategoria(catB.Id);

        //Act
        var dto = new LancamentoCadastroDto
        {
            Id = lanc.Id,
            Data = SystemTime.Today().AddDays(1),
            Descricao = "Rendimentos",
            IdCategoria = catB.Id,
            IdSubcategoria = subB.Id,
            IdMeioPagamento = mei.Id,
            Valor = 80
        };

        var response = await HttpClient.PostAsJsonAsync("/lancamento/alterar", dto);

        //Assert
        var idLancamento = await response.ConverteResultOk<int>();

        Assert.True(idLancamento > 0);

        var ldb = await Context.Lancamentos.FindAsync(idLancamento);

        Assert.NotNull(lanc);
        Assert.Equal(TestConstants.USUARIO_COMUM_ID, ldb!.IdUsuario);
        Assert.Equal(dto.IdMeioPagamento, ldb.IdMeioPagamento);
        Assert.Equal(TipoLancamento.Receita, ldb.Tipo);
        Assert.Equal(OperacaoLancamento.LancamentoSimples, ldb.Operacao);
        Assert.Null(lanc.TipoTransferencia);
        Assert.Equal(dto.Data, ldb!.Data);
        Assert.Equal(dto.Descricao, ldb.Descricao);
        Assert.Equal(dto.IdCategoria, ldb.IdCategoria);
        Assert.Equal(dto.IdSubcategoria, ldb.IdSubcategoria);
        Assert.True(ldb.Realizado);
        Assert.Null(ldb.IdLancamentoTransferencia);
        Assert.True(ldb.Valor > 0);  //lançamento de Receita é sempre positivo
        Assert.Equal(80, ldb.Valor);

        var conta = await Context.MeiosPagamento.FindAsync(mei.Id);

        Assert.NotNull(conta);
        Assert.Equal(TestConstants.USUARIO_COMUM_ID, conta!.IdUsuario);
        Assert.Equal(470, conta.Saldo);  //se o valor muda, o saldo atualiza
    }

    [Fact]
    public async Task Alterar_Receita_CartaoCredito_ValorMudou_DadosAlterados_SaldoDeCartaoEhSempreZero()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var cat = await DbSeeder.InserirCategoria(tipo: TipoLancamento.Receita);
        var sub = await DbSeeder.InserirSubcategoria(cat.Id);
        var mei = await DbSeeder.InserirMeioPagamento(saldo: 0, tipo: TipoMeioPagamento.CartaoCredito);
        var lanc = await DbSeeder.InserirLancamentoSimples(mei.Id, cat.Id, sub.Id, valor: 3800, tipo: TipoLancamento.Receita);

        var catB = await DbSeeder.InserirCategoria(nome: "Diversos", tipo: TipoLancamento.Receita);
        var subB = await DbSeeder.InserirSubcategoria(catB.Id);

        //Act
        var dto = new LancamentoCadastroDto
        {
            Id = lanc.Id,
            Data = SystemTime.Today().AddDays(1),
            Descricao = "Rendimentos",
            IdCategoria = catB.Id,
            IdSubcategoria = subB.Id,
            IdMeioPagamento = mei.Id,
            Valor = 3900
        };

        var response = await HttpClient.PostAsJsonAsync("/lancamento/alterar", dto);

        //Assert
        var idLancamento = await response.ConverteResultOk<int>();

        Assert.True(idLancamento > 0);

        var ldb = await Context.Lancamentos.FindAsync(idLancamento);

        Assert.NotNull(lanc);
        Assert.Equal(TestConstants.USUARIO_COMUM_ID, ldb!.IdUsuario);
        Assert.Equal(dto.IdMeioPagamento, ldb.IdMeioPagamento);
        Assert.Equal(TipoLancamento.Receita, ldb.Tipo);
        Assert.Equal(OperacaoLancamento.LancamentoSimples, ldb.Operacao);
        Assert.Null(lanc.TipoTransferencia);
        Assert.Equal(dto.Data, ldb!.Data);
        Assert.Equal(dto.Descricao, ldb.Descricao);
        Assert.Equal(dto.IdCategoria, ldb.IdCategoria);
        Assert.Equal(dto.IdSubcategoria, ldb.IdSubcategoria);
        Assert.True(ldb.Realizado);
        Assert.Null(ldb.IdLancamentoTransferencia);
        Assert.True(ldb.Valor > 0);  //lançamento de Receita é sempre positivo
        Assert.Equal(3900, ldb.Valor);

        var conta = await Context.MeiosPagamento.FindAsync(mei.Id);

        Assert.NotNull(conta);
        Assert.Equal(TestConstants.USUARIO_COMUM_ID, conta!.IdUsuario);
        Assert.Equal(0, conta.Saldo);  //saldo de cartão de crédito é sempre zero
    }
}
