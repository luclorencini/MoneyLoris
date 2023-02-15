namespace MoneyLoris.Application.Business.Categorias.Dtos;
public class SubcategoriaCadastroDto
{
    public int Id { get; set; }
    public int IdCategoria { get; set; }
    public string Nome { get; set; } = default!;
    public int? Ordem { get; set; }
}
