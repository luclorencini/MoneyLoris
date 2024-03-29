﻿using MoneyLoris.Application.Domain.Enums;

namespace MoneyLoris.Application.Reports.LancamentosCategoria.Dto;
public class ReportLancamentoFilterDto
{
    public int Ano { get; set; }
    public int Mes { get; set; }
    public int Quantidade { get; set; }
    public RegimeContabil Regime { get; set; }
}