using MoneyLoris.Application.Utils;

namespace MoneyLoris.Tests.Unit;
public class DateExtensionsTests
{
    [Fact]
    public void UltimoDiaMes_MesCom30Dias_RetornaUltimoDia()
    {
        //Arrange
        var date = new DateTime(2023, 9, 13); //setembro

        //Act
        var ult = date.UltimoDiaMes();

        //Assert
        Assert.Equal(2023, ult.Year);
        Assert.Equal(9, ult.Month);
        Assert.Equal(30, ult.Day);
    }

    [Fact]
    public void UltimoDiaMes_MesCom31Dias_RetornaUltimoDia()
    {
        //Arrange
        var date = new DateTime(2023, 5, 22); //maio

        //Act
        var ult = date.UltimoDiaMes();

        //Assert
        Assert.Equal(2023, ult.Year);
        Assert.Equal(5, ult.Month);
        Assert.Equal(31, ult.Day);
    }

    [Fact]
    public void UltimoDiaMes_Fevereiro_RetornaUltimoDia()
    {
        //Arrange
        var date = new DateTime(2023, 2, 18); //fevereiro

        //Act
        var ult = date.UltimoDiaMes();

        //Assert
        Assert.Equal(2023, ult.Year);
        Assert.Equal(2, ult.Month);
        Assert.Equal(28, ult.Day);
    }

    [Fact]
    public void ToSqlStringYMD_DiaMesMenorQue10_RetornaFormatado()
    {
        //Arrange
        var date = new DateTime(2023, 8, 5);

        //Act
        var sf = date.ToSqlStringYMD();

        //Assert
        Assert.NotNull(sf);
        Assert.Equal("2023-08-05", sf);
    }

    [Fact]
    public void ToSqlStringYMD_DiaMesMaiorQue10_RetornaFormatado()
    {
        //Arrange
        var date = new DateTime(2023, 10, 24);

        //Act
        var sf = date.ToSqlStringYMD();

        //Assert
        Assert.NotNull(sf);
        Assert.Equal("2023-10-24", sf);
    }
}
