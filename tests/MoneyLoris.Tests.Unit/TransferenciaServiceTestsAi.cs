using MoneyLoris.Application.Business.Auth.Interfaces;
using MoneyLoris.Application.Business.Lancamentos;
using MoneyLoris.Application.Business.Lancamentos.Dtos;
using MoneyLoris.Application.Business.Lancamentos.Interfaces;
using MoneyLoris.Application.Business.MeiosPagamento.Interfaces;
using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Shared;
using Moq;

namespace MoneyLoris.Application.Tests.Business.Lancamentos;

public class TransferenciaServiceTestsAi
{
    private readonly Mock<ILancamentoRepository> _lancamentoRepoMock;
    private readonly Mock<IMeioPagamentoRepository> _meioPagamentoRepoMock;
    private readonly Mock<IAuthenticationManager> _authenticationManagerMock;
    private readonly TransferenciaService _transferenciaService;

    public TransferenciaServiceTestsAi()
    {
        _lancamentoRepoMock = new Mock<ILancamentoRepository>();
        _meioPagamentoRepoMock = new Mock<IMeioPagamentoRepository>();
        _authenticationManagerMock = new Mock<IAuthenticationManager>();
        _transferenciaService = new TransferenciaService(_lancamentoRepoMock.Object, _meioPagamentoRepoMock.Object, _authenticationManagerMock.Object);
    }

    [Fact]
    public async Task InserirTransferenciaTransactional_ThrowsException_WhenMeioOrigemIsNull()
    {
        // Arrange
        var dto = new TransferenciaInsertDto { IdMeioPagamentoOrigem = 1, IdMeioPagamentoDestino = 2, Valor = 100, Data = DateTime.Now };
        var userInfo = new UserAuthInfo { Id = 1 };
        _authenticationManagerMock.Setup(x => x.ObterInfoUsuarioLogado()).Returns(userInfo);
        _meioPagamentoRepoMock.Setup(x => x.GetById(dto.IdMeioPagamentoOrigem)).ReturnsAsync((MeioPagamento)null!);

        // Act + Assert
        await Assert.ThrowsAsync<BusinessException>(() => _transferenciaService.InserirTransferenciaEntreContas(dto));
    }

    [Fact]
    public async Task InserirTransferenciaTransactional_ThrowsException_WhenMeioOrigemDoesNotBelongToUser()
    {
        // Arrange
        var dto = new TransferenciaInsertDto { IdMeioPagamentoOrigem = 1, IdMeioPagamentoDestino = 2, Valor = 100, Data = DateTime.Now };
        var userInfo = new UserAuthInfo { Id = 1 };
        var meioOrigem = new MeioPagamento { IdUsuario = 2 };
        _authenticationManagerMock.Setup(x => x.ObterInfoUsuarioLogado()).Returns(userInfo);
        _meioPagamentoRepoMock.Setup(x => x.GetById(dto.IdMeioPagamentoOrigem)).ReturnsAsync(meioOrigem);

        // Act + Assert
        await Assert.ThrowsAsync<BusinessException>(() => _transferenciaService.InserirTransferenciaEntreContas(dto));
    }

    [Fact]
    public async Task InserirTransferenciaTransactional_ThrowsException_WhenMeioOrigemIsCreditCard()
    {
        // Arrange
        var dto = new TransferenciaInsertDto { IdMeioPagamentoOrigem = 1, IdMeioPagamentoDestino = 2, Valor = 100, Data = DateTime.Now };
        var userInfo = new UserAuthInfo { Id = 1 };
        var meioOrigem = new MeioPagamento { IdUsuario = userInfo.Id, Tipo = TipoMeioPagamento.CartaoCredito };
        _authenticationManagerMock.Setup(x => x.ObterInfoUsuarioLogado()).Returns(userInfo);
        _meioPagamentoRepoMock.Setup(x => x.GetById(dto.IdMeioPagamentoOrigem)).ReturnsAsync(meioOrigem);

        // Act + Assert
        await Assert.ThrowsAsync<BusinessException>(() => _transferenciaService.InserirTransferenciaEntreContas(dto));
    }

}