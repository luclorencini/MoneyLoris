namespace MoneyLoris.Application.Business.Categorias.Dtos;
public class CategoriaAlteracaoDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = default!;
    public byte? Ordem { get; set; }
}
