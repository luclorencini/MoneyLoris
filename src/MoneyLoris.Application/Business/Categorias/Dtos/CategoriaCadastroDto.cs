namespace MoneyLoris.Application.Business.Categorias.Dtos;
public class CategoriaCadastroDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = default!;
    public int? Ordem { get; set; }
}
