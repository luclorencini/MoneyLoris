using Microsoft.AspNetCore.Mvc;
using MoneyLoris.Application.Business.Categorias.Interfaces;
using MoneyLoris.Application.Business.MeiosPagamento.Interfaces;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Web.Controllers.Base;

namespace MoneyLoris.Web.Controllers;

[Route("[controller]/{action=index}")]
public class CommonController : BaseController
{
    private readonly IMeioPagamentoService _meioPagamentoService;
    private readonly ICategoriaService _categoriaService;

    public CommonController(IMeioPagamentoService meioPagamentoService, ICategoriaService categoriaService)
    {
        _meioPagamentoService = meioPagamentoService;
        _categoriaService = categoriaService;
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

    [HttpGet("{tipo}")]
    public async Task<IActionResult> Categorias(TipoLancamento tipo)
    {
        var ret = await _categoriaService.ObterCategoriasUsuario(tipo);
        return Ok(ret);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Categoria(int id)
    {
        var ret = await _categoriaService.ObterCategoria(id);
        return Ok(ret);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Subcategoria(int id)
    {
        var ret = await _categoriaService.ObterSubcategoria(id);
        return Ok(ret);
    }
}
