using MoneyLoris.Application.Common.Base;
using MoneyLoris.Application.Domain.Enums;

namespace MoneyLoris.Application.Domain.Entities;

public partial class MeioPagamento : EntityBase
{
    public MeioPagamento()
    {
        Lancamentos = new HashSet<Lancamento>();
    }

    public int IdUsuario { get; set; }
    public string Nome { get; set; } = null!;
    public TipoMeioPagamento Tipo { get; set; }
    public string Cor { get; set; } = null!;
    public sbyte? Ordem { get; set; }
    public bool Ativo { get; set; }
    public decimal? Saldo { get; set; }
    public decimal? Limite { get; set; }
    public sbyte? DiaFechamento { get; set; }
    public sbyte? DiaVencimento { get; set; }

    public virtual Usuario Usuario { get; set; } = null!;
    public virtual ICollection<Lancamento> Lancamentos { get; set; }
}
