using MoneyLoris.Application.Business.Lancamentos.Dtos;
using MoneyLoris.Application.Domain.Entities;

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

    void NaoPodeTrocarMeioPagamento(Lancamento lancamento, LancamentoCadastroDto dto);
}
