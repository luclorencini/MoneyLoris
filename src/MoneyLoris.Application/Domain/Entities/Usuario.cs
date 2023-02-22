using MoneyLoris.Application.Domain.Enums;

namespace MoneyLoris.Application.Domain.Entities;

public partial class Usuario
{
    public Usuario()
    {
        Categorias = new HashSet<Categoria>();
        MeiosPagamento = new HashSet<MeioPagamento>();
    }

    public int Id { get; set; }
    public string Nome { get; set; } = null!;
    public string Login { get; set; } = null!;
    public string Senha { get; set; } = null!;
    public bool Ativo { get; set; }
    public PerfilUsuario IdPerfil { get; set; }
    public bool AlterarSenha { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? UltimoLogin { get; set; }
    public DateTime? DataInativacao { get; set; }

    public virtual ICollection<Categoria> Categorias { get; set; }
    public virtual ICollection<MeioPagamento> MeiosPagamento { get; set; }
}
