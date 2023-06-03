using MoneyLoris.Application.Business.Lancamentos.Dtos;
using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Domain.Enums;

namespace MoneyLoris.Application.Business.Lancamentos.Interfaces;
public interface ILancamentoConverter
{
    Lancamento Converter(LancamentoCadastroDto dto, TipoLancamento tipo, DateTime? data = null, string descricao = null!, decimal? valor = null, short? parcelaAtual = null, short? parcelaTotal = null);

    decimal AjustaValorLancamento(TipoLancamento tipo, decimal valor);
}
