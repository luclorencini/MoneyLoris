using MoneyLoris.Application.Business.Usuarios.Dtos;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Business.Usuarios;
public class UsuarioService : IUsuarioService
{
    public Task<Result<int>> AlterarUsuario(UsuarioAlteracaoDto dto)
    {
        //TODO
        throw new NotImplementedException();
    }

    public Task<Result<int>> CriarUsuario(UsuarioCriacaoInputDto dto)
    {
        //TODO
        throw new NotImplementedException();
    }

    public Task<Result<int>> ExcluirUsuario(int idUsuario)
    {
        //TODO
        throw new NotImplementedException();
    }

    public Task<Result<int>> InativarUsuario(int idUsuario)
    {
        //TODO
        throw new NotImplementedException();
    }

    public Task<Result<int>> MarcarParaRedefinirSenha(int idUsuario)
    {
        //TODO
        throw new NotImplementedException();
    }

    public Task<Result<UsuarioInfoDto>> ObterUsuario(int idUsuario)
    {
        //TODO
        throw new NotImplementedException();
    }

    public Task<Result<Pagination<ICollection<UsuarioListItemDto>>>> Pesquisar(UsuarioPesquisaDto filtro)
    {
        //TODO
        throw new NotImplementedException();
    }

    public Task<Result<int>> ReativarUsuario(int idUsuario)
    {
        //TODO
        throw new NotImplementedException();
    }
}
