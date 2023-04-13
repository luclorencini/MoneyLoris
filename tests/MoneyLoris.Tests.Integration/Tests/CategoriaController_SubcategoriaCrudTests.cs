using System.Net.Http.Json;
using MoneyLoris.Application.Business.Categorias.Dtos;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Shared;
using MoneyLoris.Tests.Integration.Setup.Utils;
using MoneyLoris.Tests.Integration.Tests.Base;

namespace MoneyLoris.Tests.Integration.Tests;
public class CategoriaController_SubcategoriaCrudTests : IntegrationTestsBase
{
    [Fact]
    public async Task Inserir_UsuarioAdmin_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Administrador);
        await DbSeeder.InserirUsuarios();

        var cat = await DbSeeder.InserirCategoria();

        //Act
        var dto = new SubcategoriaCadastroDto
        {
            Nome = "Outros",
            Ordem = 1,
            IdCategoria = cat.Id
        };

        var response = await HttpClient.PostAsJsonAsync("/categoria/sub/inserir", dto);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.Categoria_AdminNaoPode);
    }

    [Fact]
    public async Task Inserir_CategoriaNaoEncontrada_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        //Act
        var dto = new SubcategoriaCadastroDto
        {
            Nome = "Outros",
            Ordem = 1,
            IdCategoria = 555
        };

        var response = await HttpClient.PostAsJsonAsync("/categoria/sub/inserir", dto);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.Categoria_NaoEncontrada);
    }

    [Fact]
    public async Task Inserir_CategoriaNaoPertenceUsuario_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var cat = await DbSeeder.InserirCategoria(idUsuario: TestConstants.USUARIO_COMUM_B_ID);

        //Act
        var dto = new SubcategoriaCadastroDto
        {
            Nome = "Outros",
            Ordem = 1,
            IdCategoria = cat.Id
        };

        var response = await HttpClient.PostAsJsonAsync("/categoria/sub/inserir", dto);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.Categoria_NaoPertenceAoUsuario);
    }

    [Fact]
    public async Task Inserir_DadosCorretos_SubcategoriaCriada()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var cat = await DbSeeder.InserirCategoria();

        //Act
        var dto = new SubcategoriaCadastroDto
        {
            Nome = "Outros",
            Ordem = 1,
            IdCategoria = cat.Id
        };

        var response = await HttpClient.PostAsJsonAsync("/categoria/sub/inserir", dto);

        //Assert
        var idSub = await response.ConverteResultOk<int>();

        var subCat = await Context.Subcategorias.FindAsync(idSub);

        Assert.NotNull(subCat);
        Assert.Equal(dto.IdCategoria, subCat.IdCategoria);
        Assert.Equal(dto.Nome, subCat!.Nome);
        Assert.Equal(dto.Ordem, subCat.Ordem);
    }


    [Fact]
    public async Task Alterar_UsuarioAdmin_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Administrador);
        await DbSeeder.InserirUsuarios();

        var cat = await DbSeeder.InserirCategoria();
        var sub = await DbSeeder.InserirSubcategoria(cat.Id);

        //Act
        var dto = new SubcategoriaCadastroDto
        {
            Id = sub.Id,
            IdCategoria = cat.Id,
            Nome = "Sub 2",
            Ordem = 2,
        };

        var response = await HttpClient.PostAsJsonAsync("/categoria/sub/alterar", dto);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.Categoria_AdminNaoPode);
    }

    [Fact]
    public async Task Alterar_CategoriaNaoEncontrada_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var cat = await DbSeeder.InserirCategoria();
        var sub = await DbSeeder.InserirSubcategoria(cat.Id);

        //Act
        var dto = new SubcategoriaCadastroDto
        {
            Id = sub.Id,
            IdCategoria = 555,
            Nome = "Sub 2",
            Ordem = 2,
        };

        var response = await HttpClient.PostAsJsonAsync("/categoria/sub/alterar", dto);

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

        var cat = await DbSeeder.InserirCategoria(idUsuario: TestConstants.USUARIO_COMUM_B_ID);
        var sub = await DbSeeder.InserirSubcategoria(cat.Id);

        //Act
        var dto = new SubcategoriaCadastroDto
        {
            Id = sub.Id,
            IdCategoria = cat.Id,
            Nome = "Sub 2",
            Ordem = 2,
        };

        var response = await HttpClient.PostAsJsonAsync("/categoria/sub/alterar", dto);

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

        //Act
        var dto = new SubcategoriaCadastroDto
        {
            Id = 555,
            IdCategoria = cat.Id,
            Nome = "Sub 2",
            Ordem = 2,
        };

        var response = await HttpClient.PostAsJsonAsync("/categoria/sub/alterar", dto);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.Subcategoria_NaoEncontrada);
    }

    [Fact]
    public async Task Alterar_SubcategoriaNaoPertenceCategoria_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var cat = await DbSeeder.InserirCategoria();
        var catB = await DbSeeder.InserirCategoria(nome: "Diversos");
        var subB = await DbSeeder.InserirSubcategoria(catB.Id);

        //Act
        var dto = new SubcategoriaCadastroDto
        {
            Id = subB.Id,
            IdCategoria = cat.Id,
            Nome = "Sub 2",
            Ordem = 2,
        };

        var response = await HttpClient.PostAsJsonAsync("/categoria/sub/alterar", dto);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.Subcategoria_NaoPertenceACategoria);
    }

    [Fact]
    public async Task Alterar_DadosCorretos_SubcategoriaAlterada()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var cat = await DbSeeder.InserirCategoria();
        var sub = await DbSeeder.InserirSubcategoria(cat.Id);

        //Act
        var dto = new SubcategoriaCadastroDto
        {
            Id = sub.Id,
            IdCategoria = cat.Id,
            Nome = "Sub 2",
            Ordem = 2,
        };

        var response = await HttpClient.PostAsJsonAsync("/categoria/sub/alterar", dto);

        //Assert
        var idSub = await response.ConverteResultOk<int>();

        var subCat = await Context.Subcategorias.FindAsync(idSub);

        Assert.NotNull(subCat);
        Assert.Equal(dto.IdCategoria, subCat!.IdCategoria);
        Assert.Equal(dto.Nome, subCat!.Nome);
        Assert.Equal(dto.Ordem, subCat.Ordem);
    }


    [Fact]
    public async Task Excluir_UsuarioAdmin_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Administrador);
        await DbSeeder.InserirUsuarios();

        var cat = await DbSeeder.InserirCategoria();
        var sub = await DbSeeder.InserirSubcategoria(cat.Id);

        //Act
        var response = await HttpClient.PostAsJsonAsync("/categoria/sub/excluir", sub.Id);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.Categoria_AdminNaoPode);
    }

    [Fact]
    public async Task Excluir_SubcategoriaNaoEncontrada_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        //Act
        var response = await HttpClient.PostAsJsonAsync("/categoria/sub/excluir", 555);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.Subcategoria_NaoEncontrada);
    }

    [Fact]
    public async Task Excluir_CategoriaNaoPertenceUsuario_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var cat = await DbSeeder.InserirCategoria(idUsuario: TestConstants.USUARIO_COMUM_B_ID);
        var sub = await DbSeeder.InserirSubcategoria(cat.Id);

        //Act
        var response = await HttpClient.PostAsJsonAsync("/categoria/sub/excluir", sub.Id);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.Categoria_NaoPertenceAoUsuario);
    }

    [Fact]
    public async Task Excluir_DadosCorretos_SubcategoriaExcluida()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        var cat = await DbSeeder.InserirCategoria();
        var sub = await DbSeeder.InserirSubcategoria(cat.Id);

        //Act
        var response = await HttpClient.PostAsJsonAsync("/categoria/sub/excluir", sub.Id);

        //Assert
        var idSub = await response.ConverteResultOk<int>();

        var subCat = await Context.Subcategorias.FindAsync(idSub);

        Assert.Null(subCat);

        var categ = await Context.Categorias.FindAsync(cat.Id);

        Assert.NotNull(categ);
    }
}
