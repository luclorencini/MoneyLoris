namespace MoneyLoris.Application.Business.Lancamentos.Interfaces;
public interface IParcelaCalculator
{
    ICollection<(decimal valor, DateTime data)> CalculaParcelas(decimal valor, short parcelas, DateTime dataInicial);
}
