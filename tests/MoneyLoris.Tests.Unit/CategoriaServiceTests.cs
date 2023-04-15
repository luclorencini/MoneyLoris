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

    private readonly Mock<ICategoriaRepository> _categoriaRepoMock;
    private readonly Mock<ISubcategoriaRepository> _subcategoriaRepoMock;
    private readonly Mock<IAuthenticationManager> _authenticationManagerMock;

    private readonly CategoriaService sut;

    public CategoriaServiceTests()
    {
        _categoriaRepoMock = new Mock<ICategoriaRepository>();
        _subcategoriaRepoMock = new Mock<ISubcategoriaRepository>();
        _authenticationManagerMock = new Mock<IAuthenticationManager>();

        sut = new CategoriaService(
            new CategoriaValidator(_authenticationManagerMock.Object),
            _categoriaRepoMock.Object, 
            _subcategoriaRepoMock.Object, 
            _authenticationManagerMock.Object);
    }

    [Fact]
    public async Task Alterar_DadosCorretos_Salva()
    {
        //Arrange
        _authenticationManagerMock.Setup(x => x.ObterInfoUsuarioLogado()).Returns(
            new UserAuthInfo { Id = 5, IsAdmin = false }
            );

        _categoriaRepoMock.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(
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
        _categoriaRepoMock.Verify(x => x.Update(It.IsAny<Categoria>()));
    }

    [Fact]
    public async Task Alterar_UsuarioAdmin_Erro()
    {
        //Arrange
        _authenticationManagerMock.Setup(x => x.ObterInfoUsuarioLogado()).Returns(
            new UserAuthInfo { Id = 5, IsAdmin = true }
            );

        _categoriaRepoMock.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(
            new Categoria { IdUsuario = 5, Tipo = TipoLancamento.Receita }
            );

        //Act & Assert
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
