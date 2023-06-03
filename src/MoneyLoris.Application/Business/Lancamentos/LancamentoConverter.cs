using MoneyLoris.Application.Business.Auth.Interfaces;
using MoneyLoris.Application.Business.Lancamentos.Dtos;
using MoneyLoris.Application.Business.Lancamentos.Interfaces;
using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Domain.Enums;

namespace MoneyLoris.Application.Business.Lancamentos;
public class LancamentoConverter : ILancamentoConverter
{
    private readonly IAuthenticationManager _authenticationManager;
    private readonly ILancamentoValidator _lancamentoValidator;

    public LancamentoConverter(
        IAuthenticationManager authenticationManager,
        ILancamentoValidator lancamentoValidator)
    {
        _authenticationManager = authenticationManager;
        _lancamentoValidator = lancamentoValidator;
    }

    public Lancamento Converter(LancamentoCadastroDto dto, TipoLancamento tipo,
        DateTime? data = null, string descricao = null!, decimal? valor = null, 
        short? parcelaAtual = null, short? parcelaTotal = null)
    {
        var userInfo = _authenticationManager.ObterInfoUsuarioLogado();

        var val = (valor.HasValue ? valor.Value : dto.Valor);

        var lancamento = new Lancamento
        {
            IdUsuario = userInfo.Id,

            IdMeioPagamento = dto.IdMeioPagamento,
            IdCategoria = dto.IdCategoria,
            IdSubcategoria = dto.IdSubcategoria,

            Tipo = tipo,

            Data = (data.HasValue ? data.Value : dto.Data),
            Descricao = (!String.IsNullOrWhiteSpace(descricao) ? descricao : dto.Descricao),

            //dto sempre manda o valor positivo. Assim, se for despesa, precisa tornar negativo

            Valor = AjustaValorLancamento(tipo, val),

            Operacao = OperacaoLancamento.LancamentoSimples,
            TipoTransferencia = null,

            Realizado = true,
            IdLancamentoTransferencia = null,

            ParcelaAtual = parcelaAtual,
            ParcelaTotal = parcelaTotal
        };

        _lancamentoValidator.EstaConsistente(lancamento);

        return lancamento;
    }

    public decimal AjustaValorLancamento(TipoLancamento tipo, decimal valor)
    {
        //despesa sempre vira negativo, receita sempre vira positivo

        if (tipo == TipoLancamento.Despesa)
        {
            if (valor <= 0) return valor; //já é negativo

            return (valor * -1); //inverte o sinal
        }

        else
        {
            if (valor >= 0) return valor;  //já é positivo

            return (valor * -1); //inverte o sinal
        }

    }
}
