using MoneyLoris.Application.Business.MeiosPagamento.Dtos;
using MoneyLoris.Application.Domain.Entities;

namespace MoneyLoris.Application.Business.MeiosPagamento.Interfaces;
public interface IMeioPagamentoValidator
{
    void NaoEhAdmin();

    void Existe(MeioPagamento meio);
    void OrigemExiste(MeioPagamento meioOrigem);
    void DestinoExiste(MeioPagamento meioDestino);

    void PertenceAoUsuario(MeioPagamento meio);
    void OrigemPertenceAoUsuario(MeioPagamento meioOrigem);
    void DestinoPertenceAoUsuario(MeioPagamento meioDestino);

    void Ativo(MeioPagamento meio);
    void EstaConsistente(MeioPagamento meio);

    void EhCartaoCredito(MeioPagamento meio);

    void NaoPodeMudarDeContaPraCartaoOuViceVersa(MeioPagamento meio, MeioPagamentoCadastroDto dto);
    Task NaoPossuiLancamentos(MeioPagamento meio);
}
