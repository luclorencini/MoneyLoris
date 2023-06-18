using MoneyLoris.Application.Domain.Entities;

namespace MoneyLoris.Application.Business.Faturas.Dtos;
public class FaturaInfoDto
{
    public int Id { get; set; }
    public int Mes { get; set; }
    public int Ano { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public DateTime DataVencimento { get; set; }
    public decimal ValorFatura { get; set; }
    public decimal ValorPago { get; set; }

    public FaturaInfoDto()
    {
    }

    public FaturaInfoDto(Fatura fatura)
    {
        Id = fatura.Id;
        Mes = fatura.Mes;
        Ano = fatura.Ano;
        DataInicio = fatura.DataInicio;
        DataFim = fatura.DataFim;
        DataVencimento = fatura.DataVencimento;
        ValorPago = fatura.ValorPago.HasValue ? fatura.ValorPago.Value : 0;

        //ValorFatura não vem da tabela fatura, ele deve ser calculado em outro lugar
    }
}
