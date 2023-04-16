using Microsoft.AspNetCore.Mvc;
using MoneyLoris.Application.Business.Lancamentos.Dtos;
using MoneyLoris.Application.Business.Lancamentos.Interfaces;
using MoneyLoris.Web.Controllers.Base;

namespace MoneyLoris.Web.Controllers;

[Route("[controller]/{action=index}")]
public class TransferenciaController : BaseController
{
    private readonly ITransferenciaService _transferenciaService;

    public TransferenciaController(ITransferenciaService transferenciaService)
    {
        _transferenciaService = transferenciaService;
    }

    #region Nova Transferência

    [HttpPost()]
    [Route("/transferencia/lancar/entreContas")]
    public async Task<IActionResult> LancarTransferenciaEntreContas([FromBody] TransferenciaInsertDto transferencia)
    {
        var ret = await _transferenciaService.InserirTransferenciaEntreContas(transferencia);
        return Ok(ret);
    }

    [HttpPost()]
    [Route("/transferencia/lancar/pagamentoFatura")]
    public async Task<IActionResult> LancarPagamentoFatura([FromBody] TransferenciaInsertDto transferencia)
    {
        var ret = await _transferenciaService.InserirPagamentoFatura(transferencia);
        return Ok(ret);
    }

    #endregion

    #region Cadastro (alterar/excluir)

    [HttpGet("{id}")]
    public async Task<IActionResult> Obter(int id)
    {
        var ret = await _transferenciaService.Obter(id);
        return Ok(ret);
    }

    [HttpPost()]
    public async Task<IActionResult> Alterar([FromBody] TransferenciaUpdateDto transferencia)
    {
        var ret = await _transferenciaService.Alterar(transferencia);
        return Ok(ret);
    }

    [HttpPost()]
    public async Task<IActionResult> Excluir([FromBody] int idLancamentoOrigem)
    {
        var ret = await _transferenciaService.Excluir(idLancamentoOrigem);
        return Ok(ret);
    }

    #endregion
}
