using MoneyLoris.Application.Business.Usuarios.Dtos;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Business.Usuarios;
public interface IUsuarioService
{
    Task<Result<Pagination<ICollection<UsuarioListItemDto>>>> Pesquisar(UsuarioPesquisaDto filtro);
    Task<Result<int>> CriarUsuario(UsuarioCriacaoInputDto dto);
    Task<Result<UsuarioInfoDto>> ObterUsuario(int idUsuario);
    Task<Result<int>> AlterarUsuario(UsuarioAlteracaoDto dto);
    Task<Result<int>> MarcarParaRedefinirSenha(int idUsuario);
    Task<Result<int>> InativarUsuario(int idUsuario);
    Task<Result<int>> ReativarUsuario(int idUsuario);
    Task<Result<int>> ExcluirUsuario(int idUsuario);
}
