﻿using MoneyLoris.Application.Business.Lancamentos.Dtos;
using MoneyLoris.Application.Common.Interfaces;
using MoneyLoris.Application.Domain.Entities;
using MoneyLoris.Application.Domain.Enums;

namespace MoneyLoris.Application.Business.Lancamentos.Interfaces;
public interface ILancamentoRepository : IRepositoryBase<Lancamento>
{
    Task<Lancamento> ObterInfo(int id);
    Task<ICollection<Lancamento>> PesquisaPaginada(LancamentoFiltroDto filtro, int idUsuario);
    Task<int> PesquisaTotalRegistros(LancamentoFiltroDto filtro, int idUsuario);
    Task<decimal> SomatorioReceitasFiltro(LancamentoFiltroDto filtro, int idUsuario);
    Task<decimal> SomatorioDespesasFiltro(LancamentoFiltroDto filtro, int idUsuario);
    Task<ICollection<Lancamento>> ObterLancamentosRecentes(int idUsuario, TipoLancamento tipo, string termoBusca);
    Task<int> QuantidadePorMeioPagamento(int idMeioPagamento);
}
