﻿using MoneyLoris.Application.Reports.LancamentosCategoria.Dto;

namespace MoneyLoris.Application.Reports.LancamentosCategoria;
public interface IReportLancamentosCategoriaService
{
    CategoriaGroupItemtoDto RelatorioLancamentosPorCategoria(int mes, int ano, int quantidade);
}
