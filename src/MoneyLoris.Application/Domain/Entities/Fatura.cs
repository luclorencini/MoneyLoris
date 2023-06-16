using MoneyLoris.Application.Common.Base;

namespace MoneyLoris.Application.Domain.Entities;

public class Fatura : EntityBase
{
    public Fatura()
    {
        Lancamentos = new HashSet<Lancamento>();
    }

    public int IdMeioPagamento { get; set; }
    public int Mes { get; set; }
    public int Ano { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public DateTime DataVencimento { get; set; }
    public decimal? ValorPago { get; set; }

    public virtual MeioPagamento MeioPagamento { get; set; } = null!;
    public virtual ICollection<Lancamento> Lancamentos { get; set; }
}
