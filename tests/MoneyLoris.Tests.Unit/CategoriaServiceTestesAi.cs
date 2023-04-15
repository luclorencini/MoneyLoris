using MoneyLoris.Application.Business.Auth.Interfaces;
using MoneyLoris.Application.Business.Categorias.Dtos;
using MoneyLoris.Application.Business.Categorias.Interfaces;
using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Shared;
using Moq;

namespace MoneyLoris.Application.Business.Categorias.Tests;

public class CategoriaServiceTestsAi
{
    private readonly Mock<ICategoriaRepository> _categoriaRepoMock;
    private readonly Mock<ISubcategoriaRepository> _subcategoriaRepoMock;
    private readonly Mock<IAuthenticationManager> _authenticationManagerMock;
    private readonly CategoriaService _categoriaService;

    public CategoriaServiceTestsAi()
    {
        _categoriaRepoMock = new Mock<ICategoriaRepository>();
        _subcategoriaRepoMock = new Mock<ISubcategoriaRepository>();
        _authenticationManagerMock = new Mock<IAuthenticationManager>();

        _categoriaService = new CategoriaService(
            new CategoriaValidator(_authenticationManagerMock.Object),
            _categoriaRepoMock.Object,
            _subcategoriaRepoMock.Object,
            _authenticationManagerMock.Object);
    }

    [Fact]
    public async Task AlterarCategoria_ThrowsException_WhenUserIsAdmin()
    {
        // Arrange
        var userInfo = new UserAuthInfo { IsAdmin = true };
        _authenticationManagerMock.Setup(x => x.ObterInfoUsuarioLogado()).Returns(userInfo);
        var dto = new CategoriaCadastroDto();

        // Act & Assert
        await Assert.ThrowsAsync<BusinessException>(() => _categoriaService.AlterarCategoria(dto));

        // Assert
        _categoriaRepoMock.Verify(x => x.Update(It.IsAny<Categoria>()), Times.Never);
    }

    [Fact]
    public async Task AlterarCategoria_ThrowsException_WhenCategoriaDoesNotBelongToUser()
    {
        // Arrange
        var userInfo = new UserAuthInfo { Id = 1 };
        _authenticationManagerMock.Setup(x => x.ObterInfoUsuarioLogado()).Returns(userInfo);
        var categoria = new Categoria { IdUsuario = 2 };
        _categoriaRepoMock.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(categoria);
        var dto = new CategoriaCadastroDto { Id = 1 };

        // Act & Assert
        await Assert.ThrowsAsync<BusinessException>(() => _categoriaService.AlterarCategoria(dto));

        // Assert
        _categoriaRepoMock.Verify(x => x.Update(It.IsAny<Categoria>()), Times.Never);
    }

    [Fact]
    public async Task AlterarCategoria_ThrowsException_WhenCategoriaTypeIsDifferent()
    {
        // Arrange
        var userInfo = new UserAuthInfo { Id = 1 };
        _authenticationManagerMock.Setup(x => x.ObterInfoUsuarioLogado()).Returns(userInfo);
        var categoria = new Categoria { IdUsuario = 1, Tipo = TipoLancamento.Despesa };
        _categoriaRepoMock.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(categoria);
        var dto = new CategoriaCadastroDto { Id = 1, Tipo = TipoLancamento.Receita };

        // Act & Assert
        await Assert.ThrowsAsync<BusinessException>(() => _categoriaService.AlterarCategoria(dto));

        // Assert
        _categoriaRepoMock.Verify(x => x.Update(It.IsAny<Categoria>()), Times.Never);
    }

    [Fact]
    public async Task AlterarCategoria_UpdatesCategoria_WhenValidData()
    {
        // Arrange
        var userInfo = new UserAuthInfo { Id = 1 };
        _authenticationManagerMock.Setup(x => x.ObterInfoUsuarioLogado()).Returns(userInfo);
        var categoria = new Categoria { IdUsuario = 1, Tipo = TipoLancamento.Despesa };
        _categoriaRepoMock.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(categoria);
        var dto = new CategoriaCadastroDto { Id = 1, Tipo = TipoLancamento.Despesa, Nome = "Nova Categoria", Ordem = 1 };

        // Act
        var result = await _categoriaService.AlterarCategoria(dto);

        // Assert
        Assert.Equal(categoria.Id, result.Value);
        Assert.Equal("Categoria alterada com sucesso.", result.Message);
        Assert.Equal(dto.Nome, categoria.Nome);
        Assert.Equal(dto.Ordem, categoria.Ordem);
        _categoriaRepoMock.Verify(x => x.Update(categoria), Times.Once);
    }
}