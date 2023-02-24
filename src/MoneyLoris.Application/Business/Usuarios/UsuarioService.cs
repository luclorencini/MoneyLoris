using MoneyLoris.Application.Business.Usuarios.Dtos;
using MoneyLoris.Application.Business.Usuarios.Interfaces;
using MoneyLoris.Application.Common.Base;
using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Business.Usuarios;
public class UsuarioService : ServiceBase, IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;

    public UsuarioService(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<Result<Pagination<ICollection<UsuarioListItemDto>>>> Pesquisar(UsuarioPesquisaDto filtro)
    {
        //pega o total
        var total = await _usuarioRepository.PesquisaTotalRegistros(filtro);

        //faz a consulta paginada
        var usuarios = await _usuarioRepository.PesquisaPaginada(filtro);

        //transforma no tipo de retorno
        ICollection<UsuarioListItemDto> ret =
            usuarios.Select(u => new UsuarioListItemDto(u)).ToList();

        return Pagination(pagedData: ret, total: total);
    }

    public async Task<Result<int>> CriarUsuario(UsuarioCriacaoInputDto dto)
    {
        var usuario = new Usuario(dto.IdPerfil, dto.Nome, dto.Login);

        usuario = await _usuarioRepository.Insert(usuario);

        return (usuario.Id, "Usuário criado com sucesso.");
    }

    public async Task<Result<UsuarioInfoDto>> ObterUsuario(int idUsuario)
    {
        var usuario = await obterUsuario(idUsuario);

        var info = new UsuarioInfoDto(usuario);

        return info;
    }

    public async Task<Result<int>> AlterarUsuario(UsuarioAlteracaoDto dto)
    {
        var usuario = await obterUsuario(dto.Id);

        usuario.AlterarUsuario(dto.Nome, dto.Login);

        await _usuarioRepository.Update(usuario);

        return (dto.Id, "Usuário alterado com sucesso.");
    }

    public async Task<Result<int>> MarcarParaRedefinirSenha(int idUsuario)
    {
        var usuario = await obterUsuario(idUsuario);

        usuario.MarcarParaRedefinirSenha();

        await _usuarioRepository.Update(usuario);

        return (idUsuario, "Senha redefinida para a padrão.");
    }

    public async Task<Result<int>> InativarUsuario(int idUsuario)
    {
        var usuario = await obterUsuario(idUsuario);

        usuario.Inativar();

        await _usuarioRepository.Update(usuario);

        return (idUsuario, "Usuário inativado com sucesso.");
    }

    public async Task<Result<int>> ReativarUsuario(int idUsuario)
    {
        var usuario = await obterUsuario(idUsuario);

        usuario.Reativar();

        await _usuarioRepository.Update(usuario);

        return (idUsuario, "Usuário reativado com sucesso.");
    }

    public async Task<Result<int>> ExcluirUsuario(int idUsuario)
    {
        await _usuarioRepository.BeginTransaction();

        try
        {
            var usuario = await obterUsuario(idUsuario);

            if (usuario.IdPerfil == PerfilUsuario.Usuario)
            {
                //validacao

                //TODO - checar se ja tem lançamento, se tiver nao deixa

                // se nao tiver, apaga contas, categorias e subcategorias
                
            }

            await _usuarioRepository.Delete(usuario.Id);

            await _usuarioRepository.CommitTransaction();

            return (idUsuario, "Usuário excluído com sucesso.");

        }
        catch (Exception)
        {
            await _usuarioRepository.RollbackTransaction();
            throw;
        }
    }


    private async Task<Usuario> obterUsuario(int id)
    {
        var usuario = await _usuarioRepository.GetById(id);

        if (usuario == null)
            throw new BusinessException(
                code: ErrorCodes.Usuario_NaoEncontrado,
                message: "Usuário não encontrado");

        return usuario;
    }
}
