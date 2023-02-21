using Microsoft.AspNetCore.Mvc;
using MoneyLoris.Application.Business.Lancamentos;
using MoneyLoris.Application.Business.Lancamentos.Dtos;
using MoneyLoris.Web.Controllers.Base;

namespace MoneyLoris.Web.Controllers;

[Route("[controller]/{action=index}")]
public class TransferenciaController : BaseController
{
    private readonly ILancamentoService _lancamentoService;

    public TransferenciaController(ILancamentoService lancamentoService)
    {
        _lancamentoService = lancamentoService;
    }

    #region Nova Transferência

    [HttpPost()]
    [Route("/transferencia/lancar/entreContas")]
    public async Task<IActionResult> LancarTransferenciaEntreContas([FromBody] TransferenciaInsertDto transferencia)
    {
        var ret = await _lancamentoService.InserirTransferenciaEntreContas(transferencia);
        return Ok(ret);
    }

    [HttpPost()]
    [Route("/transferencia/lancar/pagamentoFatura")]
    public async Task<IActionResult> LancarPagamentoFatura([FromBody] TransferenciaInsertDto transferencia)
    {
        var ret = await _lancamentoService.InserirPagamentoFatura(transferencia);
        return Ok(ret);
    }

    #endregion

    #region Cadastro (alterar/excluir)

    [HttpGet("{id}")]
    public async Task<IActionResult> Obter(int id)
    {
        var ret = await _lancamentoService.ObterTransferencia(id);
        return Ok(ret);
    }

    [HttpPost()]
    public async Task<IActionResult> Alterar([FromBody] TransferenciaUpdateDto transferencia)
    {
        var ret = await _lancamentoService.AlterarTransferencia(transferencia);
        return Ok(ret);
    }

    [HttpPost()]
    public async Task<IActionResult> Excluir([FromBody] int idLancamentoOrigem)
    {
        var ret = await _lancamentoService.ExcluirTransferencia(idLancamentoOrigem);
        return Ok(ret);
    }

    #endregion
}
