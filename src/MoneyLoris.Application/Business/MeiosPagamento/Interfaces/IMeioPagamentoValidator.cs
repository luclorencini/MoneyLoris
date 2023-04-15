using MoneyLoris.Application.Business.MeiosPagamento.Dtos;
using MoneyLoris.Application.Domain.Entities;

namespace MoneyLoris.Application.Business.MeiosPagamento.Interfaces;
public interface IMeioPagamentoValidator
{
    void NaoEhAdmin();
    void Existe(MeioPagamento meio);
    void PertenceAoUsuario(MeioPagamento meio);
    void EstaConsistente(MeioPagamento meio);

    void NaoPodeMudarDeContaPraCartaoOuViceVersa(MeioPagamento meio, MeioPagamentoCadastroDto dto);
    Task NaoPossuiLancamentos(MeioPagamento meio);
}
