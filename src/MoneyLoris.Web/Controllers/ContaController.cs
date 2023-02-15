using Microsoft.AspNetCore.Mvc;
using MoneyLoris.Application.Business.Contas;
using MoneyLoris.Application.Business.Contas.Dtos;
using MoneyLoris.Web.Controllers.Base;

namespace MoneyLoris.Web.Controllers;

[Route("[controller]/{action=index}")]
public class ContaController : BaseController
{
    private readonly IContaService _contaService;

    public ContaController(IContaService contaService)
    {
        _contaService = contaService;
    }

    public IActionResult Index()
    {
        return View("ContaIndex");
    }

    [HttpGet()]
    public async Task<IActionResult> Listar()
    {
        var ret = await _contaService.Listar();
        return Ok(ret);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Obter(int id)
    {
        var ret = await _contaService.Obter(id);
        return Ok(ret);
    }

    [HttpPost()]
    public async Task<IActionResult> Inserir([FromBody] ContaCriacaoDto conta)
    {
        var ret = await _contaService.Inserir(conta);
        return Ok(ret);
    }

    [HttpPost()]
    public async Task<IActionResult> Alterar([FromBody] ContaCadastroDto conta)
    {
        var ret = await _contaService.Alterar(conta);
        return Ok(ret);
    }

    [HttpPost()]
    public async Task<IActionResult> Excluir([FromBody] int id)
    {
        var ret = await _contaService.Excluir(id);
        return Ok(ret);
    }

    [HttpPost()]
    public async Task<IActionResult> Inativar([FromBody] int id)
    {
        var ret = await _contaService.Inativar(id);
        return Ok(ret);
    }

    [HttpPost()]
    public async Task<IActionResult> Reativar([FromBody] int id)
    {
        var ret = await _contaService.Reativar(id);
        return Ok(ret);
    }
}
