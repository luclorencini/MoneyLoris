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

    public async Task<Result<LoginRetornoDto>> Login(LoginInputDto dto)
    {
        var usuario = await buscaUsuario(dto.Login, dto.Senha);

        var ret = await FazerLogin(usuario, dto.ManterConectado, dto.Pwa);
        
        return ret;
    }

    public async Task<Result<LoginRetornoDto>> LoginPwa(string pwaKey)
    {
        //TODO - por agora tá pegando o id do usuário, seguido de um guid, só pra disfarçar
        //no futuro fazer com guid e data de expiração no banco de dados

        string[] chunks = pwaKey.Split("-");
        var id = chunks[0];

        var usuario = await _usuarioRepository.GetById(Convert.ToInt32(id));

        var ret = await FazerLogin(usuario, manterConectado: true, isPwa: true);

        return ret;
    }

    private async Task<LoginRetornoDto> FazerLogin(Usuario usuario, bool manterConectado, bool isPwa)
    {
        Usuario.FazerLogin(usuario);

        if (usuario.AlterarSenha)
        {
            //se precisa alterar senha, nao faz login, e sinaliza que a troca de senha deve ser feita
            return new LoginRetornoDto { AlterarSenha = true };
        }

        //realiza o login
        await _authManager.Login(usuario, manterConectado);

        //persiste a data de login
        await _usuarioRepository.Update(usuario);

        //define url de retorno baseado no perfil
        var urlRedir = (usuario.IdPerfil == Domain.Enums.PerfilUsuario.Administrador ? "usuario" : "lancamento");

        var retorno = new LoginRetornoDto { UrlRedir = urlRedir };

        //se for pwa, retorna tambem o token de autenticação pwa
        //pra agora, retorna o id do usuario, seguido de um guid, só pra disfarçar
        //TODO - elaborar melhor no futuro, e talvez levar para authmanager
        if (isPwa)
        {
            retorno.PwaToken = $"{usuario.Id.ToString()}-{Guid.NewGuid()}" ;
        }

        return retorno;
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
        var hashPw = HashHelper.ComputeHash(senha.Trim());

        var usuario = await _usuarioRepository.GetByLoginSenha(login.Trim(), hashPw);

        return usuario;
    }

    public async Task<Result> LogOut()
    {
        await _authManager.LogOut();

        return Result.Success();
    }
}
