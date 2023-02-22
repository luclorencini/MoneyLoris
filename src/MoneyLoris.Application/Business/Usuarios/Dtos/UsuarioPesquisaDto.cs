using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Business.Usuarios.Dtos;
public class UsuarioPesquisaDto : PaginationFilter
{
    public string Nome { get; set; } = default!;

    public bool? Ativo { get; set; } = default!;

    public PerfilUsuario? IdPerfil { get; set; } = default!;
}