using MoneyLoris.Application.Common.Base;
using MoneyLoris.Application.Domain.Enums;

namespace MoneyLoris.Application.Domain.Entities;
public partial class Categoria : EntityBase
{
    public Categoria()
    {
        Lancamentos = new HashSet<Lancamento>();
        Subcategorias = new HashSet<Subcategoria>();
    }

    public TipoLancamento Tipo { get; set; }
    public int IdUsuario { get; set; }
    public string Nome { get; set; } = null!;
    public byte? Ordem { get; set; }

    public virtual Usuario Usuario { get; set; } = null!;
    public virtual ICollection<Lancamento> Lancamentos { get; set; }
    public virtual ICollection<Subcategoria> Subcategorias { get; set; }
}
