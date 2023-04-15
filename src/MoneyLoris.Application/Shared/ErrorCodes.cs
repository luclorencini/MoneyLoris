namespace MoneyLoris.Application.Shared;
public static class ErrorCodes
{
    public static readonly string SystemError = "SYSERR";

    #region Login
    public static readonly string Login_UsuarioOuSenhaNaoConferem = "LGUSNC";
    public static readonly string Login_UsuarioInativo = "LGUSIN";
    #endregion

    #region Usuario
    public static readonly string Usuario_NaoEncontrado = "USNAEN";
    public static readonly string Usuario_CamposObrigatorios = "USUCOB";
    public static readonly string Usuario_JaMarcadoParaAlterarSenha = "USJMAS";
    public static readonly string Usuario_JaEstaInativo = "USUJEI";
    public static readonly string Usuario_JaEstaAtivo = "USUJEA";
    public static readonly string Usuario_SenhaInvalida = "USUSIN";
    public static readonly string Usuario_SenhaIgualAnterior = "USUSIA";
    #endregion

    #region Categoria
    public static readonly string Categoria_AdminNaoPode = "CAADNP";
    public static readonly string Categoria_NaoEncontrada = "CANAEN";
    public static readonly string Categoria_NaoPertenceAoUsuario = "CANPUS";
    public static readonly string Categoria_NaoPodeAlterarTipo = "CANPAT";

    public static readonly string Subcategoria_NaoEncontrada = "SCNAEN";
    public static readonly string Subcategoria_NaoPertenceACategoria = "SCNPCA";

    #endregion

    #region Categoria
    public static readonly string MeioPagamento_AdminNaoPode = "MPADNP";
    public static readonly string MeioPagamento_NaoEncontrado = "MPNAEN";
    public static readonly string MeioPagamento_NaoPertenceAoUsuario = "MPNPUS";
    public static readonly string MeioPagamento_CamposObrigatorios = "MPACOB";
    public static readonly string MeioPagamento_Inativo = "MPINTV";
    public static readonly string MeioPagamento_CartaoNaoPodeVirarConta = "MPCANC";
    public static readonly string MeioPagamento_ContaNaoPodeVirarCartao = "MPCONC";
    public static readonly string MeioPagamento_CartaoNaoPermiteOperacoesSaldo = "MPCNOS";
    public static readonly string MeioPagamento_TipoDiferenteAlteracao = "MPTDAL";
    public static readonly string MeioPagamento_PossuiLancamentos = "MPPOLA";
    #endregion

    #region Lançamento
    public static readonly string Lancamento_AdminNaoPode = "LAADNP";
    public static readonly string Lancamento_NaoEncontrado = "LANAEN";
    public static readonly string Lancamento_NaoPertenceAoUsuario = "LANPUS";
    public static readonly string Lancamento_TipoDiferenteDaCategoria = "LANTDC";
    public static readonly string Lancamento_OperacaoNaoEhTransferencia = "LAONTR";
    public static readonly string Lancamento_TransferenciaOrigemNaoPodeSerCartao = "LTONCA";
    public static readonly string Lancamento_TransferenciaEntreContasDestinoNaoPodeSerCartao = "LTCDCA";
    public static readonly string Lancamento_PagamentoFaturaDestinoNaoPodeSerConta = "LPFDCO";
    #endregion
}
