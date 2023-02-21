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

    [HttpPost()]
    public async Task<IActionResult> SugestoesReceitas([FromBody] string termoBusca)
    {
        var ret = await _lancamentoService.ObterSugestoesReceitas(termoBusca);
        return Ok(ret);
    }

    [HttpPost()]
    public async Task<IActionResult> SugestoesDespesas([FromBody] string termoBusca)
    {
        var ret = await _lancamentoService.ObterSugestoesDespesas(termoBusca);
        return Ok(ret);
    }

    [HttpPost()]
    public async Task<IActionResult> LancarReceita([FromBody] LancamentoCadastroDto lancamento)
    {
        var ret = await _lancamentoService.InserirReceita(lancamento);
        return Ok(ret);
    }

    [HttpPost()]
    public async Task<IActionResult> LancarDespesa([FromBody] LancamentoCadastroDto lancamento)
    {
        var ret = await _lancamentoService.InserirDespesa(lancamento);
        return Ok(ret);
    }

    [HttpPost()]
    public async Task<IActionResult> LancarTransferencia([FromBody] TransferenciaInsertDto transferencia)
    {
        var ret = await _lancamentoService.InserirTransferenciaEntreContas(transferencia);
        return Ok(ret);
    }

    [HttpPost()]
    public async Task<IActionResult> LancarPagamentoFatura([FromBody] TransferenciaInsertDto transferencia)
    {
        var ret = await _lancamentoService.InserirPagamentoFatura(transferencia);
        return Ok(ret);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterLancamento(int id)
    {
        var ret = await _lancamentoService.ObterLancamento(id);
        return Ok(ret);
    }

    [HttpPost()]
    public async Task<IActionResult> AlterarLancamento([FromBody] LancamentoCadastroDto lancamento)
    {
        var ret = await _lancamentoService.AlterarLancamento(lancamento);
        return Ok(ret);
    }

    [HttpPost()]
    public async Task<IActionResult> ExcluirLancamento([FromBody] int id)
    {
        var ret = await _lancamentoService.ExcluirLancamento(id);
        return Ok(ret);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterTransferencia(int id)
    {
        var ret = await _lancamentoService.ObterTransferencia(id);
        return Ok(ret);
    }

    [HttpPost()]
    public async Task<IActionResult> AlterarTransferencia([FromBody] TransferenciaUpdateDto transferencia)
    {
        var ret = await _lancamentoService.AlterarTransferencia(transferencia);
        return Ok(ret);
    }

    [HttpPost()]
    public async Task<IActionResult> ExcluirTransferencia([FromBody] int idLancamentoOrigem)
    {
        var ret = await _lancamentoService.ExcluirTransferencia(idLancamentoOrigem);
        return Ok(ret);
    }
}
