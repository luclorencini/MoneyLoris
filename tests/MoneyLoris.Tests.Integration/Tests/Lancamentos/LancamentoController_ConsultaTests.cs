using System.Net.Http.Json;
using MoneyLoris.Application.Business.Lancamentos.Dtos;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Tests.Integration.Setup.Utils;
using MoneyLoris.Tests.Integration.Tests.Base;

namespace MoneyLoris.Tests.Integration.Tests.Lancamentos;
public class LancamentoController_ConsultaTests : IntegrationTestsBase
{

    private async Task setupDadosListagem()
    {
        var mpua = await DbSeeder.InserirMeioPagamento(TestConstants.USUARIO_COMUM_ID, TipoMeioPagamento.ContaCorrente, "Caixa");
        var ctda = await DbSeeder.InserirCategoria(TipoLancamento.Despesa, TestConstants.USUARIO_COMUM_ID, "Compras");
        var ctra = await DbSeeder.InserirCategoria(TipoLancamento.Receita, TestConstants.USUARIO_COMUM_ID, "Ganhos");

        var mpub = await DbSeeder.InserirMeioPagamento(TestConstants.USUARIO_COMUM_B_ID, TipoMeioPagamento.ContaPagamento, "PicPay");
        var ctdb = await DbSeeder.InserirCategoria(TipoLancamento.Despesa, TestConstants.USUARIO_COMUM_B_ID, "Despesas Gerais");
        var ctrb = await DbSeeder.InserirCategoria(TipoLancamento.Receita, TestConstants.USUARIO_COMUM_B_ID, "Receitas Gerais");

        await DbSeeder.InserirLancamentoSimples(mpua.Id, ctda.Id, tipo: TipoLancamento.Despesa, idUsuario: TestConstants.USUARIO_COMUM_ID, data: new DateTime(2023, 04, 02), descricao: "Come come");
        await DbSeeder.InserirLancamentoSimples(mpua.Id, ctda.Id, tipo: TipoLancamento.Despesa, idUsuario: TestConstants.USUARIO_COMUM_ID, data: new DateTime(2023, 03, 30), descricao: "Extrabom");
        await DbSeeder.InserirLancamentoSimples(mpua.Id, ctda.Id, tipo: TipoLancamento.Despesa, idUsuario: TestConstants.USUARIO_COMUM_ID, data: new DateTime(2023, 03, 25), descricao: "Come come");
        await DbSeeder.InserirLancamentoSimples(mpua.Id, ctda.Id, tipo: TipoLancamento.Despesa, idUsuario: TestConstants.USUARIO_COMUM_ID, data: new DateTime(2023, 03, 28), descricao: "Shell Combustíveis");
        await DbSeeder.InserirLancamentoSimples(mpua.Id, ctda.Id, tipo: TipoLancamento.Despesa, idUsuario: TestConstants.USUARIO_COMUM_ID, data: new DateTime(2023, 03, 21), descricao: "Farmácia Santa Lúcia");
        await DbSeeder.InserirLancamentoSimples(mpua.Id, ctda.Id, tipo: TipoLancamento.Despesa, idUsuario: TestConstants.USUARIO_COMUM_ID, data: new DateTime(2023, 04, 12), descricao: "Com Você Distribuidora");

        await DbSeeder.InserirLancamentoSimples(mpua.Id, ctra.Id, tipo: TipoLancamento.Receita, idUsuario: TestConstants.USUARIO_COMUM_ID, data: new DateTime(2023, 03, 05), descricao: "Salário");
        await DbSeeder.InserirLancamentoSimples(mpua.Id, ctra.Id, tipo: TipoLancamento.Receita, idUsuario: TestConstants.USUARIO_COMUM_ID, data: new DateTime(2023, 03, 12), descricao: "Comissão");
        await DbSeeder.InserirLancamentoSimples(mpua.Id, ctra.Id, tipo: TipoLancamento.Receita, idUsuario: TestConstants.USUARIO_COMUM_ID, data: new DateTime(2023, 04, 17), descricao: "Venda da TV");
        await DbSeeder.InserirLancamentoSimples(mpua.Id, ctra.Id, tipo: TipoLancamento.Receita, idUsuario: TestConstants.USUARIO_COMUM_ID, data: new DateTime(2023, 04, 10), descricao: "Comissão");
        await DbSeeder.InserirLancamentoSimples(mpua.Id, ctra.Id, tipo: TipoLancamento.Receita, idUsuario: TestConstants.USUARIO_COMUM_ID, data: new DateTime(2023, 04, 15), descricao: "Acordo com devedores");

        await DbSeeder.InserirLancamentoSimples(mpub.Id, ctdb.Id, tipo: TipoLancamento.Despesa, idUsuario: TestConstants.USUARIO_COMUM_B_ID, data: new DateTime(2023, 04, 07), descricao: "Dallas Comércio");
        await DbSeeder.InserirLancamentoSimples(mpub.Id, ctdb.Id, tipo: TipoLancamento.Despesa, idUsuario: TestConstants.USUARIO_COMUM_B_ID, data: new DateTime(2023, 02, 27), descricao: "Carone");
        await DbSeeder.InserirLancamentoSimples(mpub.Id, ctdb.Id, tipo: TipoLancamento.Despesa, idUsuario: TestConstants.USUARIO_COMUM_B_ID, data: new DateTime(2023, 03, 15), descricao: "Pizzaria Comarella");

        await DbSeeder.InserirLancamentoSimples(mpub.Id, ctrb.Id, tipo: TipoLancamento.Receita, idUsuario: TestConstants.USUARIO_COMUM_ID, data: new DateTime(2023, 02, 05), descricao: "Salário");
        await DbSeeder.InserirLancamentoSimples(mpub.Id, ctrb.Id, tipo: TipoLancamento.Receita, idUsuario: TestConstants.USUARIO_COMUM_ID, data: new DateTime(2023, 03, 05), descricao: "Salário");

    }

    [Fact]
    public async Task ObterSugestoesDespesas_LancamentosDeVariosUsuarios_NaoInformaTermo_RetornaUltimosDoUsuario()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        await setupDadosListagem();

        //Act
        var response = await HttpClient.PostAsJsonAsync("/lancamento/sugestoes/despesas", "");

        //Assert
        var dto = await response.ConverteResultOk<ICollection<LancamentoSugestaoDto>>();

        Assert.NotNull(dto);
        Assert.Equal(5, dto.Count);

        var arr = dto.ToArray();

        Assert.Equal("Com Você Distribuidora", arr[0].Descricao);
        Assert.Equal("Come come", arr[1].Descricao);
        Assert.Equal("Extrabom", arr[2].Descricao);
        Assert.Equal("Shell Combustíveis", arr[3].Descricao);
        Assert.Equal("Farmácia Santa Lúcia", arr[4].Descricao);
    }

    [Fact]
    public async Task ObterSugestoesDespesas_LancamentosDeVariosUsuarios_InformaTermoQueTemResultado_RetornaUltimosDoUsuarioQueBatemComTermo()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        await setupDadosListagem();

        //Act
        var response = await HttpClient.PostAsJsonAsync("/lancamento/sugestoes/despesas", "com");

        //Assert
        var dto = await response.ConverteResultOk<ICollection<LancamentoSugestaoDto>>();

        Assert.NotNull(dto);
        Assert.Equal(3, dto.Count);

        var arr = dto.ToArray();

        Assert.Equal("Com Você Distribuidora", arr[0].Descricao);
        Assert.Equal("Come come", arr[1].Descricao);
        Assert.Equal("Shell Combustíveis", arr[2].Descricao);
    }

    [Fact]
    public async Task ObterSugestoesReceitas_LancamentosDeVariosUsuarios_NaoInformaTermo_RetornaUltimosDoUsuario()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        await setupDadosListagem();

        //Act
        var response = await HttpClient.PostAsJsonAsync("/lancamento/sugestoes/receitas", "");

        //Assert
        var dto = await response.ConverteResultOk<ICollection<LancamentoSugestaoDto>>();

        Assert.NotNull(dto);
        Assert.Equal(5, dto.Count);

        var arr = dto.ToArray();

        Assert.Equal("Venda da TV", arr[0].Descricao);
        Assert.Equal("Acordo com devedores", arr[1].Descricao);
        Assert.Equal("Comissão", arr[2].Descricao);
        Assert.Equal("Salário", arr[3].Descricao);
    }

    [Fact]
    public async Task ObterSugestoesReceitas_LancamentosDeVariosUsuarios_InformaTermoQueTemResultado_RetornaUltimosDoUsuarioQueBatemComTermo()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        await setupDadosListagem();

        //Act
        var response = await HttpClient.PostAsJsonAsync("/lancamento/sugestoes/receitas", "com");

        //Assert
        var dto = await response.ConverteResultOk<ICollection<LancamentoSugestaoDto>>();

        Assert.NotNull(dto);
        Assert.Equal(2, dto.Count);

        var arr = dto.ToArray();

        Assert.Equal("Acordo com devedores", arr[0].Descricao);
        Assert.Equal("Comissão", arr[1].Descricao);
    }
}
