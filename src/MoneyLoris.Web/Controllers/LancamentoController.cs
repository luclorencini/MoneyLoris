using Microsoft.AspNetCore.Mvc;
using MoneyLoris.Application.Business.Lancamentos;
using MoneyLoris.Application.Business.Lancamentos.Dtos;
using MoneyLoris.Web.Controllers.Base;

namespace MoneyLoris.Web.Controllers;

[Route("[controller]/{action=index}")]
public class LancamentoController : BaseController
{
    private readonly ILancamentoService _lancamentoService;

    public LancamentoController(ILancamentoService lancamentoService)
    {
        _lancamentoService = lancamentoService;
    }

    public IActionResult Index()
    {
        return View("LancamentoIndex");
    }

    [HttpPost()]
    public async Task<IActionResult> Pesquisar([FromBody] LancamentoFiltroDto filtro)
    {
        var ret = await _lancamentoService.Pesquisar(filtro);
        return Ok(ret);
    }

    [HttpPost()]
    public async Task<IActionResult> Balanco([FromBody] LancamentoFiltroDto filtro)
    {
        var ret = await _lancamentoService.ObterBalanco(filtro);
        return Ok(ret);
    }
}
