using Microsoft.AspNetCore.Mvc;
using MoneyLoris.Application.Business.MeiosPagamento;
using MoneyLoris.Web.Controllers.Base;

namespace MoneyLoris.Web.Controllers;

[Route("[controller]/{action=index}")]
public class CommonController : BaseController
{
    private readonly IMeioPagamentoService _meioPagamentoService;

    public CommonController(IMeioPagamentoService meioPagamentoService)
    {
        _meioPagamentoService = meioPagamentoService;
    }

    [HttpGet()]
    public async Task<IActionResult> MeiosPagamento()
    {
        var ret = await _meioPagamentoService.ObterMeiosPagamento();
        return Ok(ret);
    }

    [HttpGet()]
    public async Task<IActionResult> Contas()
    {
        var ret = await _meioPagamentoService.ObterContas();
        return Ok(ret);
    }

    [HttpGet()]
    public async Task<IActionResult> Cartoes()
    {
        var ret = await _meioPagamentoService.ObterCartoes();
        return Ok(ret);
    }

    [HttpGet()]
    public async Task<IActionResult> Categorias()
    {
        //var ret = await _meioPagamentoService.ObterContas();
        //return Ok(ret);
        return Ok();
    }
}
