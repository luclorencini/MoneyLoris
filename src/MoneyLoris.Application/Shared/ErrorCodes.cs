namespace MoneyLoris.Application.Shared;
public static class ErrorCodes
{
    public static readonly string SystemError = "SYSERR";

    #region Login
    public static readonly string Login_UsuarioOuSenhaNaoConferem = "LGUSNC";
    public static readonly string Login_UsuarioInativo = "LGUSIN";
    #endregion

    #region Usuario
    //TODO - renomear
    public static readonly string Conta_NaoEncontrado = "CONAEN";
    public static readonly string Conta_CamposObrigatorios = "CONCOB";
    public static readonly string Conta_JaMarcadaParaAlterarSenha = "COJMAS";
    public static readonly string Conta_JaEstaInativa = "CONJEI";
    public static readonly string Conta_JaEstaAtiva = "CONJEA";
    public static readonly string Conta_PsicologoPossuiPaciente = "CEXPPP";
    public static readonly string Conta_SenhaInvalida = "CONSIN";
    public static readonly string Conta_SenhaIgualAnterior = "CONSIA";
    #endregion

    
}
