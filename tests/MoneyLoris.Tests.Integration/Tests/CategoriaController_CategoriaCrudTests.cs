using System.Net.Http.Json;
using MoneyLoris.Application.Business.Categorias.Dtos;
using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Shared;
using MoneyLoris.Tests.Integration.Setup.Utils;
using MoneyLoris.Tests.Integration.Tests.Base;

namespace MoneyLoris.Tests.Integration.Tests;
public class CategoriaController_CategoriaCrudTests : IntegrationTestsBase
{
    [Fact]
    public async Task Inserir_UsuarioAdmin_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Administrador);
        await DbSeeder.InserirUsuarios();

        //Act
        var dto = new CategoriaCadastroDto
        {
            Nome = "Outros",
            Ordem = 1,
            Tipo = TipoLancamento.Receita
        };

        var response = await HttpClient.PostAsJsonAsync("/categoria/inserir", dto);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.Categoria_AdminNaoPode);
    }

    [Fact]
    public async Task Inserir_DadosCorretos_ComOrdem_CategoriaCriada()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        //Act
        var dto = new CategoriaCadastroDto
        {
            Nome = "Pessoal",
            Ordem = 1,
            Tipo = TipoLancamento.Receita
        };

        var response = await HttpClient.PostAsJsonAsync("/categoria/inserir", dto);

        //Assert
        var idCateg = await response.ConverteResultOk<int>();

        var categ = await Context.Categorias.FindAsync(idCateg);

        Assert.NotNull(categ);
        Assert.Equal(dto.Nome, categ!.Nome);
        Assert.Equal(dto.Ordem, categ.Ordem);
        Assert.Equal(dto.Tipo, categ.Tipo);
    }

    [Fact]
    public async Task Inserir_DadosCorretos_SemOrdem_CategoriaCriada()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        //Act
        var dto = new CategoriaCadastroDto
        {
            Nome = "Pessoal",
            Ordem = null, //sem ordem informada
            Tipo = TipoLancamento.Despesa
        };

        var response = await HttpClient.PostAsJsonAsync("/categoria/inserir", dto);

        //Assert
        var idCateg = await response.ConverteResultOk<int>();

        var categ = await Context.Categorias.FindAsync(idCateg);

        Assert.NotNull(categ);
        Assert.Equal(dto.Nome, categ!.Nome);
        Assert.Null(categ.Ordem);
        Assert.Equal(dto.Tipo, categ.Tipo);
    }


    private async Task<Categoria> inserirCategoria(int? IdUsuarioDonoCategoria = null)
    {
        var ent = await Context.Categorias.AddAsync(
            new Categoria
            {
                Nome = "Coisas",
                Ordem = null,
                Tipo = TipoLancamento.Receita,
                IdUsuario = IdUsuarioDonoCategoria ?? TestConstants.USUARIO_COMUM_ID
            });

        await Context.SaveChangesAsync();

        return ent.Entity;
    }


    [Fact]
    public async Task Alterar_UsuarioAdmin_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Administrador);
        await DbSeeder.InserirUsuarios();

        await inserirCategoria();

        //Act
        var dto = new CategoriaCadastroDto
        {
            Id = 1,
            Nome = "Coisas 2",
            Ordem = 1,
            Tipo = TipoLancamento.Receita
        };

        var response = await HttpClient.PostAsJsonAsync("/categoria/alterar", dto);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.Categoria_AdminNaoPode);
    }

    [Fact]
    public async Task Alterar_CategoriaNaoPertenceUsuario_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        await inserirCategoria(TestConstants.USUARIO_COMUM_B_ID);

        //Act
        var dto = new CategoriaCadastroDto
        {
            Id = 1,
            Nome = "Coisas 2",
            Ordem = 1,
            Tipo = TipoLancamento.Receita
        };

        var response = await HttpClient.PostAsJsonAsync("/categoria/alterar", dto);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.Categoria_NaoPertenceAoUsuario);
    }

    [Fact]
    public async Task Alterar_UsuarioAlterouTipo_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        await inserirCategoria();

        //Act
        var dto = new CategoriaCadastroDto
        {
            Id = 1,
            Nome = "Coisas 2",
            Ordem = 1,
            Tipo = TipoLancamento.Despesa
        };

        var response = await HttpClient.PostAsJsonAsync("/categoria/alterar", dto);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.Categoria_NaoPodeAlterarTipo);
    }

    [Fact]
    public async Task Alterar_DadosCorretos_CategoriaAlterada()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        await inserirCategoria();

        //Act
        var dto = new CategoriaCadastroDto
        {
            Id = 1,
            Nome = "Coisas 2",
            Ordem = 1,
            Tipo = TipoLancamento.Receita
        };

        var response = await HttpClient.PostAsJsonAsync("/categoria/alterar", dto);

        //Assert
        var idCateg = await response.ConverteResultOk<int>();

        var categ = await Context.Categorias.FindAsync(idCateg);

        Assert.NotNull(categ);
        Assert.Equal(dto.Nome, categ!.Nome);
        Assert.Equal(dto.Ordem, categ.Ordem);
        Assert.Equal(dto.Tipo, categ.Tipo);
    }



    [Fact]
    public async Task Excluir_UsuarioAdmin_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Administrador);
        await DbSeeder.InserirUsuarios();

        await inserirCategoria();

        //Act
        var response = await HttpClient.PostAsJsonAsync("/categoria/excluir", 1);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.Categoria_AdminNaoPode);
    }

    [Fact]
    public async Task Excluir_CategoriaNaoPertenceUsuario_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        await inserirCategoria(TestConstants.USUARIO_COMUM_B_ID);

        //Act
        var response = await HttpClient.PostAsJsonAsync("/categoria/excluir", 1);

        //Assert
        await response.AssertResultNotOk(
            ErrorCodes.Categoria_NaoPertenceAoUsuario);
    }

    [Fact]
    public async Task Excluir_DadosCorretos_CategoriaExcluida()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);
        await DbSeeder.InserirUsuarios();

        await inserirCategoria();

        //Act
        var response = await HttpClient.PostAsJsonAsync("/categoria/excluir", 1);

        //Assert
        var idCateg = await response.ConverteResultOk<int>();

        var categ = await Context.Categorias.FindAsync(idCateg);

        Assert.Null(categ);
    }
}
