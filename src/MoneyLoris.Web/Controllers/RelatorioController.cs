using Microsoft.AspNetCore.Mvc;
using MoneyLoris.Application.Reports.LancamentosCategoria;
using MoneyLoris.Application.Reports.LancamentosCategoria.Dto;
using MoneyLoris.Web.Controllers.Base;

namespace MoneyLoris.Web.Controllers;

[Route("[controller]/{action=index}")]
public class RelatorioController : BaseController
{
    private readonly IReportLancamentosCategoriaService _reportService;

    public RelatorioController(IReportLancamentosCategoriaService repo)
    {
        _reportService = repo;
    }

    public IActionResult Index()
    {
        return View("~/Views/Relatorio/LancamentoCategoria/LancamentoCategoriaIndex.cshtml");
    }

    [HttpPost]
    public IActionResult Pesquisar([FromBody] ReportLancamentoFilterDto filtro)
    {
        var ret = _reportService.LancamentosPorCategoriaConsolidado(filtro);
        return Ok(ret);
    }

    [HttpPost]
    public async Task<IActionResult> DetalheListagem([FromBody] ReportLancamentoDetalheFilterDto filtro)
    {
        var ret = await _reportService.PesquisarDetalhe(filtro);
        return Ok(ret);
    }

    [HttpPost]
    public async Task<IActionResult> DetalheSomatorio([FromBody] ReportLancamentoDetalheFilterDto filtro)
    {
        var ret = await _reportService.ObterDetalheSomatorio(filtro);
        return Ok(ret);
    }


}
