namespace MoneyLoris.Application.Domain.Entities;

public partial class Subcategoria
{
    public Subcategoria()
    {
        Lancamentos = new HashSet<Lancamento>();
    }

    public int Id { get; set; }
    public int IdCategoria { get; set; }
    public string Nome { get; set; } = null!;
    public sbyte? Ordem { get; set; }

    public virtual Categoria Categoria { get; set; } = null!;
    public virtual ICollection<Lancamento> Lancamentos { get; set; }
}
