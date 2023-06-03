using MoneyLoris.Application.Common.Base;
using MoneyLoris.Application.Domain.Enums;

namespace MoneyLoris.Application.Domain.Entities;

public partial class Lancamento : EntityBase
{
    public Lancamento()
    {
        LancamentoTransferenciaRelacionado = new HashSet<Lancamento>();
    }

    public int IdUsuario { get; set; }
    public int IdMeioPagamento { get; set; }
    public TipoLancamento Tipo { get; set; }
    public OperacaoLancamento Operacao { get; set; }
    public TipoTransferencia? TipoTransferencia { get; set; }
    public DateTime Data { get; set; }
    public string Descricao { get; set; } = null!;
    public decimal Valor { get; set; }
    public int? IdCategoria { get; set; }
    public int? IdSubcategoria { get; set; }
    public bool? Realizado { get; set; }
    public int? IdLancamentoTransferencia { get; set; }
    public short? ParcelaAtual { get; set; }
    public short? ParcelaTotal { get; set; }


    public virtual Usuario Usuario { get; set; } = null!;
    public virtual MeioPagamento MeioPagamento { get; set; } = null!;
    public virtual Categoria? Categoria { get; set; }
    public virtual Subcategoria? Subcategoria { get; set; }
    public virtual Lancamento? LancamentoTransferencia { get; set; }

    //TODO - preciso mesmo dessa navegação?
    public virtual ICollection<Lancamento> LancamentoTransferenciaRelacionado { get; set; }
}
