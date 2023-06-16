using MoneyLoris.Application.Business.Faturas;
using MoneyLoris.Application.Domain.Entities;

namespace MoneyLoris.Tests.Unit;
public class FaturaFactoryTests
{
    private MeioPagamento criarCartao(byte fecha, byte vence)
    {
        return new MeioPagamento
        {
            Id = 123,
            Tipo = Application.Domain.Enums.TipoMeioPagamento.CartaoCredito,
            DiaFechamento = fecha,
            DiaVencimento = vence
        };
    }

    [Fact]
    public void Criar_DatasNormais_OK()
    {
        //Arrange
        var cartao = criarCartao(fecha: 3, vence: 10);

        var sut = new FaturaFactory();

        //Act
        var fatura = sut.Criar(cartao, 6, 2023);

        //Assert
        Assert.NotNull(fatura);
        Assert.Equal(123, fatura.IdMeioPagamento);
        Assert.Equal(6, fatura.Mes);
        Assert.Equal(2023, fatura.Ano);
        Assert.Equal(new DateTime(2023, 6, 10), fatura.DataVencimento);
        Assert.Equal(new DateTime(2023, 5, 3), fatura.DataInicio);
        Assert.Equal(new DateTime(2023, 6, 2), fatura.DataFim);
        Assert.Null(fatura.ValorPago);
    }

    [Fact]
    public void Criar_ViradaDeAno_OK()
    {
        //Arrange
        var cartao = criarCartao(fecha: 5, vence: 15);

        var sut = new FaturaFactory();

        //Act
        var fatura = sut.Criar(cartao, 1, 2024);

        //Assert
        Assert.NotNull(fatura);
        Assert.Equal(123, fatura.IdMeioPagamento);
        Assert.Equal(1, fatura.Mes);
        Assert.Equal(2024, fatura.Ano);
        Assert.Equal(new DateTime(2024, 1, 15), fatura.DataVencimento);
        Assert.Equal(new DateTime(2023, 12, 5), fatura.DataInicio);
        Assert.Equal(new DateTime(2024, 1, 4), fatura.DataFim);
        Assert.Null(fatura.ValorPago);
    }
}
