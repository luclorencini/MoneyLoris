using Microsoft.AspNetCore.Mvc;
using MoneyLoris.Application.Business.Faturas.Dtos;
using MoneyLoris.Application.Business.Faturas.Interfaces;

namespace MoneyLoris.Web.Controllers;

[Route("[controller]/{action=index}")]
public class FaturaController : Controller
{
    private readonly IFaturaService _faturaService;

    public FaturaController(IFaturaService faturaService)
    {
        _faturaService = faturaService;
    }

    public IActionResult Index()
    {
        return View("FaturaIndex");
    }

    [HttpPost()]
    public async Task<IActionResult> Info([FromBody] FaturaFiltroDto filtro)
    {
        var ret = await _faturaService.ObterInfo(filtro);
        return Ok(ret);
    }

    [HttpPost()]
    public async Task<IActionResult> Pesquisar([FromBody] FaturaFiltroDto filtro)
    {
        var ret = await _faturaService.Pesquisar(filtro);
        return Ok(ret);
    }


}
