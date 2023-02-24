namespace MoneyLoris.Application.Shared;
public static class ErrorCodes
{
    public static readonly string SystemError = "SYSERR";

    #region Login
    public static readonly string Login_UsuarioOuSenhaNaoConferem = "LGUSNC";
    public static readonly string Login_UsuarioInativo = "LGUSIN";
    #endregion

    #region Usuario
    public static readonly string Usuario_NaoEncontrado = "USUAEN";
    public static readonly string Usuario_CamposObrigatorios = "USUCOB";
    public static readonly string Usuario_JaMarcadoParaAlterarSenha = "COJMAS";
    public static readonly string Usuario_JaEstaInativo = "USUJEI";
    public static readonly string Usuario_JaEstaAtivo = "USUJEA";
    public static readonly string Usuario_SenhaInvalida = "USUSIN";
    public static readonly string Usuario_SenhaIgualAnterior = "USUSIA";
    #endregion

    
}
