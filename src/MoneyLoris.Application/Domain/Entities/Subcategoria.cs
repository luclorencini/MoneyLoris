﻿using MoneyLoris.Application.Common.Base;

namespace MoneyLoris.Application.Domain.Entities;

public partial class Subcategoria : EntityBase
{
    public Subcategoria()
    {
        Lancamentos = new HashSet<Lancamento>();
    }

    public int IdCategoria { get; set; }
    public string Nome { get; set; } = null!;
    public byte? Ordem { get; set; }

    public virtual Categoria Categoria { get; set; } = null!;
    public virtual ICollection<Lancamento> Lancamentos { get; set; }
}
