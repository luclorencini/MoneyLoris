using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Domain.Enums;

namespace MoneyLoris.Application.Business.Lancamentos.Interfaces;
public interface ILancamentoValidator
{
    void NaoEhAdmin();
    void Existe(Lancamento lancamento);
    void OrigemExiste(Lancamento lancamentoOrigem);
    void DestinoExiste(Lancamento lancamentoDestino);

    void PertenceAoUsuario(Lancamento lancamento);
    void OrigemPertenceAoUsuario(Lancamento lancamentoOrigem);
    void DestinoPertenceAoUsuario(Lancamento lancamentoDestino);


    void EstaConsistente(Lancamento lancamento);

    void LancamentoCartaoCreditoTemQueTerParcela(MeioPagamento meio, short? parcelas);
    void LancamentoCartaoCreditoTemQueTerParcela(MeioPagamento meio, short? parcelaAtual, short? parcelaTotal);
    void LancamentoCartaoCreditoTemQueTerFatura(MeioPagamento meio, int? mesFatura, int? anoFatura);

    void TipoLancamentoIgualTipoCategoria(TipoLancamento tipo, Categoria categoria);
    void NaoPodeTrocarMeioPagamento(Lancamento lancamento, int idMeioPagamentoInformado);
}
