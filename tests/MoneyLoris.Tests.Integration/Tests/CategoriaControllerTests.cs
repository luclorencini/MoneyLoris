using System.Net.Http.Json;
using MoneyLoris.Application.Business.Categorias.Dtos;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Shared;
using MoneyLoris.Tests.Integration.Setup.Utils;
using MoneyLoris.Tests.Integration.Tests.Base;

namespace MoneyLoris.Tests.Integration.Tests;
public class CategoriaControllerTests : IntegrationTestsBase
{
    [Fact]
    public async Task Inserir_UsuarioAdmin_RetornaErro()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Administrador);

        await DbSeed.InserirUsuarios();

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
    public async Task Inserir_TipoReceita_DadosCorretos_CategoriaCriada()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);

        await DbSeed.InserirUsuarios();

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
    public async Task Inserir_TipoDespesa_DadosCorretos_CategoriaCriada()
    {
        //Arrange
        SubirAplicacao(perfil: PerfilUsuario.Usuario);

        await DbSeed.InserirUsuarios();

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
}
