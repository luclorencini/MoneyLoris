using MoneyLoris.Application.Business.Lancamentos;

namespace MoneyLoris.Tests.Unit;
public class ParcelaCalculatorTests
{
    private void assertParcela((decimal valor, DateTime data) p,
        decimal valorEsperado, DateTime dataEsperada)
    {
        Assert.Equal(valorEsperado, p.valor);
        Assert.Equal(dataEsperada, p.data);
    }


    [Fact]
    public void CalculaParcelas_ApenasUmaParcela_RetornaListaComUmElemento()
    {
        //Arrange
        var DATA_INICIAL = new DateTime(2023, 3, 25);
        var sut = new ParcelaCalculator();

        //Act
        var list = sut.CalculaParcelas(100, 1, DATA_INICIAL);

        //Assert
        Assert.NotNull(list);
        Assert.Equal(1, list.Count);

        var p = list.First();

        assertParcela(p, 100, DATA_INICIAL);
    }

    [Fact]
    public void CalculaParcelas_ParcelasIguais_Inteiro_RetornaLista()
    {
        //Arrange
        var sut = new ParcelaCalculator();

        //Act
        var list = sut.CalculaParcelas(300, 2, new DateTime(2023, 4, 12));

        //Assert
        Assert.NotNull(list);
        Assert.Equal(2, list.Count);

        var arr = list.ToArray();

        assertParcela(arr[0], 150, new DateTime(2023, 4, 12));
        assertParcela(arr[1], 150, new DateTime(2023, 5, 12));
    }

    [Fact]
    public void CalculaParcelas_ParcelasIguais_Decimal_RetornaLista()
    {
        //Arrange
        var sut = new ParcelaCalculator();

        //Act
        var list = sut.CalculaParcelas(229.60m, 2, new DateTime(2023, 2, 15));

        //Assert
        Assert.NotNull(list);
        Assert.Equal(2, list.Count);

        var arr = list.ToArray();

        assertParcela(arr[0], 114.80m, new DateTime(2023, 2, 15));
        assertParcela(arr[1], 114.80m, new DateTime(2023, 3, 15));
    }

    [Fact]
    public void CalculaParcelas_ParcelamentoGeraValoresDiferentes_Dois_RetornaLista()
    {
        //Arrange
        var sut = new ParcelaCalculator();

        //Act
        var list = sut.CalculaParcelas(284.99m, 2, new DateTime(2022, 10, 10));

        //Assert
        Assert.NotNull(list);
        Assert.Equal(2, list.Count);

        var arr = list.ToArray();

        assertParcela(arr[0], 142.50m, new DateTime(2022, 10, 10));
        assertParcela(arr[1], 142.49m, new DateTime(2022, 11, 10));
    }

    [Fact]
    public void CalculaParcelas_ParcelamentoGeraValoresDiferentes_Tres_RetornaLista()
    {
        //Arrange
        var sut = new ParcelaCalculator();

        //Act
        var list = sut.CalculaParcelas(968.90m, 3, new DateTime(2023, 2, 10));

        //Assert
        Assert.NotNull(list);
        Assert.Equal(3, list.Count);

        var arr = list.ToArray();

        assertParcela(arr[0], 322.98m, new DateTime(2023, 2, 10));
        assertParcela(arr[1], 322.96m, new DateTime(2023, 3, 10));
        assertParcela(arr[2], 322.96m, new DateTime(2023, 4, 10));
    }

    [Fact]
    public void CalculaParcelas_ParcelamentoGeraValoresDiferentes_Dez_RetornaLista()
    {
        //Arrange
        var sut = new ParcelaCalculator();

        //Act
        var list = sut.CalculaParcelas(2659.98m, 10, new DateTime(2019, 3, 8));

        //Assert
        Assert.NotNull(list);
        Assert.Equal(10, list.Count);

        var arr = list.ToArray();

        assertParcela(arr[0], 266.07m, new DateTime(2019, 3, 8));
        assertParcela(arr[1], 265.99m, new DateTime(2019, 4, 8));
        assertParcela(arr[2], 265.99m, new DateTime(2019, 5, 8));
        assertParcela(arr[3], 265.99m, new DateTime(2019, 6, 8));
        assertParcela(arr[4], 265.99m, new DateTime(2019, 7, 8));
        assertParcela(arr[5], 265.99m, new DateTime(2019, 8, 8));
        assertParcela(arr[6], 265.99m, new DateTime(2019, 9, 8));
        assertParcela(arr[7], 265.99m, new DateTime(2019, 10, 8));
        assertParcela(arr[8], 265.99m, new DateTime(2019, 11, 8));
        assertParcela(arr[9], 265.99m, new DateTime(2019, 12, 8));
    }
}
