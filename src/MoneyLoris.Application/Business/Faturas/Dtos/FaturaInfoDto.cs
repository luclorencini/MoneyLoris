namespace MoneyLoris.Application.Business.Faturas.Dtos;
public class FaturaInfoDto
{
    public int Mes { get; set; }
    public int Ano { get; set; }
    public DateTime DataFechamento { get; set; }
    public DateTime DataVencimento { get; set; }
    public decimal ValorFatura { get; set; }
    public decimal ValorPago { get; set; }
}
