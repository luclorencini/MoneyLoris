using MoneyLoris.Application.Business.Auth.Interfaces;
using MoneyLoris.Application.Business.Categorias;
using MoneyLoris.Application.Business.Categorias.Dtos;
using MoneyLoris.Application.Business.Categorias.Interfaces;
using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Shared;
using Moq;

namespace MoneyLoris.Tests.Unit;
public class CategoriaServiceTests
{

    private readonly Mock<ICategoriaRepository> catRepo;
    private readonly Mock<ISubcategoriaRepository> subRepo;
    private readonly Mock<IAuthenticationManager> authMan;

    private readonly CategoriaService sut;

    public CategoriaServiceTests()
    {
        catRepo = new Mock<ICategoriaRepository>();
        subRepo = new Mock<ISubcategoriaRepository>();
        authMan = new Mock<IAuthenticationManager>();

        sut = new CategoriaService(catRepo.Object, subRepo.Object, authMan.Object);
    }

    [Fact]
    public async Task Alterar_DadosCorretos_Salva()
    {
        //Arrange
        authMan.Setup(x => x.ObterInfoUsuarioLogado()).Returns(
            new UserAuthInfo { Id = 5, IsAdmin = false }
            );

        catRepo.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(
            new Categoria { IdUsuario = 5, Tipo = TipoLancamento.Receita }
            );

        //Act
        var dto = new CategoriaCadastroDto
        {
            Nome = "Pessoal",
            Ordem = 1,
            Tipo = TipoLancamento.Receita
        };

        var ret = await sut.AlterarCategoria(dto);

        //Assert
        catRepo.Verify(x => x.Update(It.IsAny<Categoria>()));
    }

    [Fact]
    public async Task Alterar_UsuarioAdmin_Erro()
    {
        //Arrange
        authMan.Setup(x => x.ObterInfoUsuarioLogado()).Returns(
            new UserAuthInfo { Id = 5, IsAdmin = true }
            );

        catRepo.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(
            new Categoria { IdUsuario = 5, Tipo = TipoLancamento.Receita }
            );

        //Act
        var dto = new CategoriaCadastroDto
        {
            Nome = "Pessoal",
            Ordem = 1,
            Tipo = TipoLancamento.Receita
        };

        var ex = await Assert.ThrowsAsync<BusinessException>(
            async () => await sut.AlterarCategoria(dto)
            );

        //Assert
        Assert.Equal(ErrorCodes.Categoria_AdminNaoPode, ex.ErrorCode);
    }

    //[Fact]
    //public async Task Alterar_CategoriaNaoPertenceUsuario_Erro()
    //{

    //}

    //[Fact]
    //public async Task Alterar_UsuarioAlterouTipo_Erro()
    //{

    //}

    //[Fact]
    //public async Task Alterar_NomeNaoInformado_Erro()
    //{

    //}
}
