using MoneyLoris.Application.Business.Categorias.Dtos;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Tests.Integration.Setup.Utils;
using MoneyLoris.Tests.Integration.Tests.Base;

namespace MoneyLoris.Tests.Integration.Tests;
public class CategoriaController_ListagemTests : IntegrationTestsBase
{
    [Fact]
    public async Task Listar_NenhumaCategoriaDoUsuario_RetornaListaVazia()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        await inserirCategoriasESubcategoriasParaOutroUsuario(TipoLancamento.Despesa);

        //Act
        var response = await HttpClient.GetAsync("/categoria/listar/despesas");

        //Assert
        var lista = await response.ConverteResultOk<ICollection<CategoriaCadastroListItemDto>>();

        Assert.NotNull(lista);
        Assert.Empty(lista);
    }

    [Fact]
    public async Task Listar_CategoriasSemSubcategorias_SemOrdem_RetornaLista()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        await inserirCategoriasESubcategoriasParaOutroUsuario(TipoLancamento.Receita);

        var catA = await DbSeeder.InserirCategoria(nome: "Cat A", tipo: TipoLancamento.Receita);
        var catB = await DbSeeder.InserirCategoria(nome: "Cat B", tipo: TipoLancamento.Receita);
        var catC = await DbSeeder.InserirCategoria(nome: "Cat C", tipo: TipoLancamento.Receita);

        //Act
        var response = await HttpClient.GetAsync("/categoria/listar/receitas");

        //Assert
        var lista = await response.ConverteResultOk<ICollection<CategoriaCadastroListItemDto>>();

        Assert.NotNull(lista);
        Assert.Equal(3, lista.Count);

        var arr = lista.ToArray();

        Assert.Equal("Cat A", arr[0].Nome);
        Assert.Equal(0, arr[0].Subcategorias.Count);

        Assert.Equal("Cat B", arr[1].Nome);
        Assert.Equal(0, arr[1].Subcategorias.Count);

        Assert.Equal("Cat C", arr[2].Nome);
        Assert.Equal(0, arr[2].Subcategorias.Count);
    }

    [Fact]
    public async Task Listar_CategoriasSemSubcategorias_ComOrdem_RetornaLista()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        await inserirCategoriasESubcategoriasParaOutroUsuario(TipoLancamento.Receita);

        var catA = await DbSeeder.InserirCategoria(nome: "Cat A", tipo: TipoLancamento.Receita, ordem: 2);
        var catB = await DbSeeder.InserirCategoria(nome: "Cat B", tipo: TipoLancamento.Receita);
        var catC = await DbSeeder.InserirCategoria(nome: "Cat C", tipo: TipoLancamento.Receita, ordem: 1);

        //Act
        var response = await HttpClient.GetAsync("/categoria/listar/receitas");

        //Assert
        var lista = await response.ConverteResultOk<ICollection<CategoriaCadastroListItemDto>>();

        Assert.NotNull(lista);
        Assert.Equal(3, lista.Count);

        var arr = lista.ToArray();

        Assert.Equal("Cat C", arr[0].Nome);
        Assert.Equal(0, arr[0].Subcategorias.Count);

        Assert.Equal("Cat A", arr[1].Nome);
        Assert.Equal(0, arr[1].Subcategorias.Count);

        Assert.Equal("Cat B", arr[2].Nome);
        Assert.Equal(0, arr[2].Subcategorias.Count);
    }

    [Fact]
    public async Task Listar_CategoriasCemSubcategorias_ComESemOrdem_RetornaLista()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        await inserirCategoriasESubcategoriasParaOutroUsuario(TipoLancamento.Despesa);

        var catA = await DbSeeder.InserirCategoria(nome: "Cat A", ordem: 2);
        var subA3 = await DbSeeder.InserirSubcategoria(catA.Id, "Sub A3");
        var subA2 = await DbSeeder.InserirSubcategoria(catA.Id, "Sub A2");
        var subA1 = await DbSeeder.InserirSubcategoria(catA.Id, "Sub A1");

        var catB = await DbSeeder.InserirCategoria(nome: "Cat B", ordem: 1);

        var catC = await DbSeeder.InserirCategoria(nome: "Cat C");
        var subC4 = await DbSeeder.InserirSubcategoria(catC.Id, "Sub C4", 1);
        var subC2 = await DbSeeder.InserirSubcategoria(catC.Id, "Sub C2", 1);
        var subC3 = await DbSeeder.InserirSubcategoria(catC.Id, "Sub C3", 2);
        var subC1 = await DbSeeder.InserirSubcategoria(catC.Id, "Sub C1");

        var catD = await DbSeeder.InserirCategoria(nome: "Cat D", ordem: 1);
        var subD1 = await DbSeeder.InserirSubcategoria(catD.Id, "Sub D1");

        //Act
        var response = await HttpClient.GetAsync("/categoria/listar/despesas");

        //Assert
        var lista = await response.ConverteResultOk<ICollection<CategoriaCadastroListItemDto>>();

        Assert.NotNull(lista);
        Assert.Equal(4, lista.Count);

        var arr = lista.ToArray();

        var cb = arr[0];
        Assert.Equal("Cat B", cb.Nome);
        Assert.Equal(0, cb.Subcategorias.Count);

        var cd = arr[1];
        Assert.Equal("Cat D", cd.Nome);
        Assert.Equal(1, cd.Subcategorias.Count);
        var sd = cd.Subcategorias.ToArray();
        Assert.Equal("Sub D1", sd[0].Nome);

        var ca = arr[2];
        Assert.Equal("Cat A", ca.Nome);
        Assert.Equal(3, ca.Subcategorias.Count);
        var sa = ca.Subcategorias.ToArray();
        Assert.Equal("Sub A1", sa[0].Nome);
        Assert.Equal("Sub A2", sa[1].Nome);
        Assert.Equal("Sub A3", sa[2].Nome);

        var cc = arr[3];
        Assert.Equal("Cat C", cc.Nome);
        Assert.Equal(4, cc.Subcategorias.Count);
        var sc = cc.Subcategorias.ToArray();
        Assert.Equal("Sub C2", sc[0].Nome);
        Assert.Equal("Sub C4", sc[1].Nome);
        Assert.Equal("Sub C3", sc[2].Nome);
        Assert.Equal("Sub C1", sc[3].Nome);
    }

    private async Task inserirCategoriasESubcategoriasParaOutroUsuario(TipoLancamento tipo)
    {
        var catU1 = await DbSeeder.InserirCategoria(nome: "Cat UserB 1", idUsuario: TestConstants.USUARIO_COMUM_B_ID, tipo: tipo);
        var subU1 = await DbSeeder.InserirSubcategoria(idCategoria: catU1.Id, nome: "SubCat UserB 1");

        var catU2 = await DbSeeder.InserirCategoria(nome: "Cat UserB 2", idUsuario: TestConstants.USUARIO_COMUM_B_ID, tipo: tipo);
        var subU2 = await DbSeeder.InserirSubcategoria(idCategoria: catU2.Id, nome: "SubCat UserB 2");

        var catU3 = await DbSeeder.InserirCategoria(nome: "Cat UserB 3", idUsuario: TestConstants.USUARIO_COMUM_B_ID, tipo: tipo);
        var subU3 = await DbSeeder.InserirSubcategoria(idCategoria: catU3.Id, nome: "SubCat UserB 3");
    }
}
