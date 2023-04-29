using MoneyLoris.Application.Business.Lancamentos.Interfaces;

namespace MoneyLoris.Application.Business.Lancamentos;
public class ParcelaCalculator : IParcelaCalculator
{
    /// <summary>
    /// Nos casos em que o resultado da divisão do valor da venda pela quantidade de parcelas for uma dízima periódica, o valor da primeira parcela será maior do que as demais.
    /// </summary>
    /// <param name="valor"></param>
    /// <param name="parcelas"></param>
    /// <param name="dataInicial"></param>
    /// <returns></returns>
    public ICollection<(decimal valor, DateTime data)> CalculaParcelas(decimal valor, short parcelas, DateTime dataInicial)
    {
        var parcList = new List<(decimal, DateTime)>();

        var vals = CalculaArredondamentoParcelas(valor, parcelas).ToArray();

        for (int i = 0; i < parcelas; i++)
        {
            var p = (vals[i], dataInicial.AddMonths(i));
            parcList.Add(p);
        }

        return parcList;
    }

    private IEnumerable<decimal> CalculaArredondamentoParcelas(decimal valor, short parcelas)
    {
        //se só tem 1 parcela, retorna o valor original imediatamente
        if (parcelas == 1) return new List<decimal> { valor };


        var list = new List<decimal>();

        //faz a divisão das parcelas
        var val = valor / parcelas;

        //arredonda para 2 casas decimais para cortar eventuais dizimas periodicas
        var valR = Math.Round(val, 2, MidpointRounding.ToZero);

        //calcula o ajuste de centavos entre os valores arredondados e o valor original
        var ajuste = valor - (valR * parcelas);

        //adiciona o ajuste na PRIMEIRA parcela

        for (int i = 1; i <= parcelas; i++)
        {
            var v = (i == 1) ? valR + ajuste : valR;
            list.Add(v);
        }

        return list;
    }
}
