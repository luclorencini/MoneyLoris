using Microsoft.AspNetCore.Mvc;
using MoneyLoris.Application.Business.Lancamentos.Dtos;
using MoneyLoris.Application.Business.Lancamentos.Interfaces;
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

    #region Novo Lançamento

    [HttpPost()]
    [Route("/lancamento/sugestoes/receitas")]
    public async Task<IActionResult> SugestoesReceitas([FromBody] string termoBusca)
    {
        var ret = await _lancamentoService.ObterSugestoesReceitas(termoBusca);
        return Ok(ret);
    }

    [HttpPost()]
    [Route("/lancamento/sugestoes/despesas")]
    public async Task<IActionResult> SugestoesDespesas([FromBody] string termoBusca)
    {
        var ret = await _lancamentoService.ObterSugestoesDespesas(termoBusca);
        return Ok(ret);
    }

    [HttpPost()]
    [Route("/lancamento/lancar/receita")]
    public async Task<IActionResult> LancarReceita([FromBody] LancamentoCadastroDto lancamento)
    {
        var ret = await _lancamentoService.InserirReceita(lancamento);
        return Ok(ret);
    }

    [HttpPost()]
    [Route("/lancamento/lancar/despesa")]
    public async Task<IActionResult> LancarDespesa([FromBody] LancamentoCadastroDto lancamento)
    {
        var ret = await _lancamentoService.InserirDespesa(lancamento);
        return Ok(ret);
    }

    #endregion

    #region Cadastro (alterar/excluir)

    [HttpGet("{id}")]
    public async Task<IActionResult> Obter(int id)
    {
        var ret = await _lancamentoService.Obter(id);
        return Ok(ret);
    }

    [HttpPost()]
    public async Task<IActionResult> Alterar([FromBody] LancamentoCadastroDto lancamento)
    {
        var ret = await _lancamentoService.Alterar(lancamento);
        return Ok(ret);
    }

    [HttpPost()]
    public async Task<IActionResult> Excluir([FromBody] int id)
    {
        var ret = await _lancamentoService.Excluir(id);
        return Ok(ret);
    }

    #endregion
}
