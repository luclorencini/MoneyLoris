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
    public static readonly string Categoria_CamposObrigatorios = "CACOBR";
    public static readonly string Categoria_NaoPodeAlterarTipo = "CANPAT";

    public static readonly string Subcategoria_NaoEncontrada = "SCNAEN";
    public static readonly string Subcategoria_NaoPertenceACategoria = "SCNPCA";
    public static readonly string Subcategoria_CamposObrigatorios = "SCCOBR";

    #endregion

    #region Categoria
    public static readonly string MeioPagamento_AdminNaoPode = "MPADNP";
    public static readonly string MeioPagamento_NaoEncontrado = "MPNAEN";
    public static readonly string MeioPagamento_OrigemNaoEncontrado = "MPONEN";
    public static readonly string MeioPagamento_DestinoNaoEncontrado = "MPDNEN";
    public static readonly string MeioPagamento_NaoPertenceAoUsuario = "MPNPUS";
    public static readonly string MeioPagamento_OrigemNaoPertenceAoUsuario = "MPONPU";
    public static readonly string MeioPagamento_DestinoNaoPertenceAoUsuario = "MPDNPU";
    public static readonly string MeioPagamento_CamposObrigatorios = "MPACOB";
    public static readonly string MeioPagamento_Inativo = "MPINTV";
    public static readonly string MeioPagamento_NaoEhCartaoCredito = "MPNECC";
    public static readonly string MeioPagamento_CartaoNaoPodeVirarConta = "MPCANC";
    public static readonly string MeioPagamento_ContaNaoPodeVirarCartao = "MPCONC";
    public static readonly string MeioPagamento_CartaoNaoPermiteOperacoesSaldo = "MPCNOS";
    public static readonly string MeioPagamento_TipoDiferenteAlteracao = "MPTDAL";
    public static readonly string MeioPagamento_PossuiLancamentos = "MPPOLA";
    #endregion

    #region Lançamento
    public static readonly string Lancamento_AdminNaoPode = "LAADNP";
    public static readonly string Lancamento_NaoEncontrado = "LANAEN";
    public static readonly string Lancamento_OrigemNaoEncontrado = "LAONAE";
    public static readonly string Lancamento_DestinoNaoEncontrado = "LAONAE";
    public static readonly string Lancamento_NaoPertenceAoUsuario = "LANPUS";
    public static readonly string Lancamento_OrigemNaoPertenceAoUsuario = "LAONPU";
    public static readonly string Lancamento_DestinoNaoPertenceAoUsuario = "LADNPU";
    public static readonly string Lancamento_CamposObrigatorios = "LACOBR";
    public static readonly string Lancamento_TipoDiferenteDaCategoria = "LANTDC";
    public static readonly string Lancamento_CartaoCreditoSemParcela = "LACCSP";
    public static readonly string Lancamento_CartaoCreditoSemFatura = "LACCSF";

    #endregion

    #region Transferencia
    public static readonly string Transferencia_AdminNaoPode = "TRADNP";
    public static readonly string Transferencia_MeioOrigemNaoPodeSerCartao = "TRONCA";
    public static readonly string Transferencia_EntreContasDestinoNaoPodeSerCartao = "TRCDNC";
    public static readonly string Transferencia_PagamentoFaturaDestinoTemQueSerCartao = "TPFDTC";
    public static readonly string Transferencia_OperacaoLancamentoOrigemNaoEhTransferencia = "TOLONT";
    public static readonly string Transferencia_OperacaoLancamentoDestinoNaoEhTransferencia = "TOLDNT";
    public static readonly string Transferencia_PagamentoFaturaAnoMesFaturaDevemSerInformados = "TPFAMF";
    #endregion

    #region Fatura
    public static readonly string Fatura_AdminNaoPode = "FAADNP";
    public static readonly string Fatura_NaoEncontrado = "FANAEN";
    #endregion
}
