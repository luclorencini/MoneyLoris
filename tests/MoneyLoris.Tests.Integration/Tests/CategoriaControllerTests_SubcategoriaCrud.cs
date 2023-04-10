using System.Net.Http.Json;
using MoneyLoris.Application.Business.Categorias.Dtos;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Shared;
using MoneyLoris.Tests.Integration.Setup.Utils;
using MoneyLoris.Tests.Integration.Tests.Base;

namespace MoneyLoris.Tests.Integration.Tests;
public class CategoriaControllerTests_SubcategoriaCrud : IntegrationTestsBase
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

}
