﻿namespace MoneyLoris.Application.Business.Categorias.Dtos;
public class CategoriaListItemDto
{
    public int IdCategoria { get; set; }
    public int? IdSubcategoria { get; set; }
    public string Nome { get; set; } = default!;
}
