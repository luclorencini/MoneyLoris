using Microsoft.AspNetCore.Mvc;
using MoneyLoris.Application.Business.MeiosPagamento;
using MoneyLoris.Application.Business.MeiosPagamento.Dtos;
using MoneyLoris.Web.Controllers.Base;

namespace MoneyLoris.Web.Controllers;

[Route("[controller]/{action=index}")]
public class ContaController : BaseController
{
    private readonly IMeioPagamentoService _meioPagamentoService;

    public ContaController(IMeioPagamentoService meioPagamentoService)
    {
        _meioPagamentoService = meioPagamentoService;
    }

    public IActionResult Index()
    {
        return View("ContaIndex");
    }

    [HttpGet()]
    public async Task<IActionResult> Listar()
    {
        var ret = await _meioPagamentoService.Listar();
        return Ok(ret);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Obter(int id)
    {
        var ret = await _meioPagamentoService.Obter(id);
        return Ok(ret);
    }

    [HttpPost()]
    public async Task<IActionResult> Inserir([FromBody] MeioPagamentoCriacaoDto conta)
    {
        var ret = await _meioPagamentoService.Inserir(conta);
        return Ok(ret);
    }

    [HttpPost()]
    public async Task<IActionResult> Alterar([FromBody] MeioPagamentoCadastroDto conta)
    {
        var ret = await _meioPagamentoService.Alterar(conta);
        return Ok(ret);
    }

    [HttpPost()]
    public async Task<IActionResult> Excluir([FromBody] int id)
    {
        var ret = await _meioPagamentoService.Excluir(id);
        return Ok(ret);
    }

    [HttpPost()]
    public async Task<IActionResult> Inativar([FromBody] int id)
    {
        var ret = await _meioPagamentoService.Inativar(id);
        return Ok(ret);
    }

    [HttpPost()]
    public async Task<IActionResult> Reativar([FromBody] int id)
    {
        var ret = await _meioPagamentoService.Reativar(id);
        return Ok(ret);
    }
}
