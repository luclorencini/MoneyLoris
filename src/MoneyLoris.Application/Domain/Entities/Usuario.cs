using MoneyLoris.Application.Common.Base;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Application.Shared;
using MoneyLoris.Application.Utils;

namespace MoneyLoris.Application.Domain.Entities;

public partial class Usuario : EntityBase
{
    public Usuario()
    {
        Categorias = new HashSet<Categoria>();
        MeiosPagamento = new HashSet<MeioPagamento>();
    }

    public string Nome { get; set; } = null!;
    public string Login { get; set; } = null!;
    public string Senha { get; set; } = null!;
    public bool Ativo { get; set; }
    public PerfilUsuario IdPerfil { get; set; }
    public bool AlterarSenha { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? UltimoLogin { get; set; }
    public DateTime? DataInativacao { get; set; }

    public virtual ICollection<Categoria> Categorias { get; set; } = default!;
    public virtual ICollection<MeioPagamento> MeiosPagamento { get; set; } = default!;
    public virtual ICollection<Lancamento> Lancamentos { get; set; } = default!;


    private readonly string SENHA_PADRAO = "dinheiro";


    /// <summary>
    /// Cria um novo usuario ativo.
    /// A usuario é criado com a senha padrão 'dinheiro', e o usuário deve trocá-la no primeiro acesso
    /// </summary>
    /// <param name="idPerfil"></param>
    /// <param name="nomeCompleto"></param>
    /// <param name="login"></param>
    /// <returns>Um Usuário pronto para ser inserido na base</returns>
    /// 
    public Usuario(PerfilUsuario idPerfil, string nomeCompleto, string login)
    {
        //validacoes
        if (string.IsNullOrWhiteSpace(nomeCompleto) ||
            string.IsNullOrWhiteSpace(login))
            throw new BusinessException(
                code: ErrorCodes.Usuario_CamposObrigatorios,
                message: "Novo Usuário - campos obrigatórios não foram informados");

        IdPerfil = idPerfil;
        Nome = nomeCompleto;
        Login = login;

        Senha = HashHelper.ComputeHash(SENHA_PADRAO);
        AlterarSenha = true; //para forçar usuário a trocar a senha no primeiro acesso

        DataCriacao = SystemTime.Now();
        Ativo = true;

        UltimoLogin = null;
        DataInativacao = null;
    }

    public void AlterarUsuario(string nomeCompleto, string login)
    {
        //validacoes
        if (string.IsNullOrWhiteSpace(nomeCompleto) ||
            string.IsNullOrWhiteSpace(login))
            throw new BusinessException(
                code: ErrorCodes.Usuario_CamposObrigatorios,
                message: "Alterar Usuário - campos obrigatórios não foram informados");

        //altera só nome e login
        Nome = nomeCompleto;
        Login = login;
    }

    public void MarcarParaRedefinirSenha()
    {
        // marca flag para forçar usuário a trocar a senha no próximo acesso.
        // A senha do usuário será trocada pela senha padrão.

        if (AlterarSenha == true)
            throw new BusinessException(
                code: ErrorCodes.Usuario_JaMarcadoParaAlterarSenha,
                message: "Este usuário já foi marcado para redefinição de senha no próximo login");

        Senha = HashHelper.ComputeHash(SENHA_PADRAO);
        AlterarSenha = true;
    }

    public void Inativar()
    {
        if (Ativo == false)
            throw new BusinessException(
                code: ErrorCodes.Usuario_JaEstaInativo,
                message: "Este usuário já se encontra inativo");

        Ativo = false;
        DataInativacao = SystemTime.Now(); //guarda a data da inativação
    }

    public void Reativar()
    {
        if (Ativo == true)
            throw new BusinessException(
                code: ErrorCodes.Usuario_JaEstaAtivo,
                message: "Este usuário já se encontra ativo");

        Ativo = true;
        DataInativacao = null; //reseta a data da inativação
    }

    public static void FazerLogin(Usuario usuario)
    {
        //veriica se usuario foi encontrado
        if (usuario is null)
            throw new BusinessException(
                code: ErrorCodes.Login_UsuarioOuSenhaNaoConferem,
                message: "Usuário ou senha não conferem.");

        //verifica se usuario está ativo
        if (!usuario.Ativo)
            throw new BusinessException(
                code: ErrorCodes.Login_UsuarioInativo,
                message: "Usuário se encontra inativo.");

        //se não precisa alterar senha, atualiza a data de ultimo login
        if (!usuario.AlterarSenha)
        {
            usuario.UltimoLogin = SystemTime.Now();
        }
    }

    public void InformarNovaSenha(string novaSenha)
    {
        if (novaSenha.Length < 6)
            throw new BusinessException(
                code: ErrorCodes.Usuario_SenhaInvalida,
                message: "Senha precisa ter no mínimo 6 caracteres");

        var novaSenhaHash = HashHelper.ComputeHash(novaSenha);

        if (novaSenhaHash == Senha)
            throw new BusinessException(
                code: ErrorCodes.Usuario_SenhaIgualAnterior,
                message: "Nova senha não pode ser igual à anterior");

        Senha = novaSenhaHash;
        AlterarSenha = false; // libera usuário para logar novamente
    }
}
