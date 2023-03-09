using Microsoft.AspNetCore.Mvc;
using MoneyLoris.Application.Business.Lancamentos.Dtos;
using MoneyLoris.Application.Reports.LancamentosCategoria;
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

    [HttpPost()]
    public IActionResult Pesquisar()
    {
        var ret = _reportService.RelatorioLancamentosPorCategoria(1, 2023, 12);
        return Ok(ret);
    }
}
