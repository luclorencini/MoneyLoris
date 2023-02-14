using Microsoft.AspNetCore.Mvc;
using MoneyLoris.Application.Business.Categorias.Interfaces;
using MoneyLoris.Application.Domain;
using MoneyLoris.Web.Controllers.Base;

namespace MoneyLoris.Web.Controllers;

public class CategoriaController : BaseController
{
    private readonly ICategoriaService _categoriaService;

    public CategoriaController(ICategoriaService categoriaService)
    {
        _categoriaService = categoriaService;
    }

    public IActionResult Index()
    {
        return View("CategoriaIndex");
    }

    [HttpGet()]
    public async Task<IActionResult> ListarCategoriasDespesas()
    {
        var ret = await _categoriaService.ListarCategoriasUsuario(Enums.TipoLancamento.Despesa);
        return Ok(ret);
    }

    [HttpGet()]
    public async Task<IActionResult> ListarCategoriasReceitas()
    {
        var ret = await _categoriaService.ListarCategoriasUsuario(Enums.TipoLancamento.Receita);
        return Ok(ret);
    }
}
