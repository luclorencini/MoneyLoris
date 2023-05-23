﻿using MoneyLoris.Application.Shared;

namespace MoneyLoris.Application.Reports.LancamentosCategoria.Dto;
public class ReportLancamentoDetalheFilterDto : PaginationFilter
{
    public int IdCategoria { get; set; }
    public int? IdSubcategoria { get; set; }
    public int Ano { get; set; }
    public int Mes { get; set; }
}