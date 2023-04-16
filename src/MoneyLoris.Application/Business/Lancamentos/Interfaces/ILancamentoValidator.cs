using MoneyLoris.Application.Business.Lancamentos.Dtos;
using MoneyLoris.Application.Domain.Entities;

namespace MoneyLoris.Application.Business.Lancamentos.Interfaces;
public interface ILancamentoValidator
{
    void NaoEhAdmin();
    void Existe(Lancamento lancamento);
    void PertenceAoUsuario(Lancamento lancamento);
    void EstaConsistente(Lancamento lancamento);

    void NaoPodeTrocarMeioPagamento(Lancamento lancamento, LancamentoCadastroDto dto);
}
