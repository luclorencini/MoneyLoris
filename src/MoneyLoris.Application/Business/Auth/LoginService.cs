using MoneyLoris.Application.Business.Auth.Dtos;
using MoneyLoris.Application.Business.Auth.Interfaces;
using MoneyLoris.Application.Business.Usuarios.Interfaces;
using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Shared;
using MoneyLoris.Application.Utils;

namespace MoneyLoris.Application.Business.Auth;
public class LoginService : ILoginService
{
    private readonly IAuthenticationManager _authManager;
    private readonly IUsuarioRepository _usuarioRepository;

    public LoginService(IUsuarioRepository usuarioRepository, IAuthenticationManager authManager)
    {
        _usuarioRepository = usuarioRepository;
        _authManager = authManager;
    }

    public async Task<Result<bool>> Login(LoginInputDto dto)
    {
        Usuario usuario = await buscaUsuario(dto.Login, dto.Senha);

        Usuario.FazerLogin(usuario);

        if (usuario.AlterarSenha)
        {
            //se precisa alterar senha, nao faz login, e sinaliza que a troca de senha deve ser feita
            return true;
        }

        //realiza o login
        await _authManager.Login(usuario, dto.ManterConectado);

        //persiste a data de login
        await _usuarioRepository.Update(usuario);

        //retorna false: não precisa alterar senha
        return false;
    }

    public async Task<Result> AlterarSenha(AlteracaoSenhaDto dto)
    {
        Usuario usuario = await buscaUsuario(dto.Login, dto.SenhaAtual);

        Usuario.FazerLogin(usuario);

        //altera a senha
        usuario.InformarNovaSenha(dto.NovaSenha);

        //persiste alteração
        await _usuarioRepository.Update(usuario);

        return Result.Success();
    }

    private async Task<Usuario> buscaUsuario(string login, string senha)
    {
        //faz o hash da senha para poder procurar na base
        var hashPw = HashHelper.ComputeHash(senha);

        var usuario = await _usuarioRepository.GetByLoginSenha(login, hashPw);

        return usuario;
    }

    public async Task<Result> LogOut()
    {
        await _authManager.LogOut();

        return Result.Success();
    }
}
