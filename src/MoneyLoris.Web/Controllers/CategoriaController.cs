using Microsoft.AspNetCore.Mvc;
using MoneyLoris.Application.Business.Categorias.Dtos;
using MoneyLoris.Application.Business.Categorias.Interfaces;
using MoneyLoris.Application.Domain.Enums;
using MoneyLoris.Web.Controllers.Base;

namespace MoneyLoris.Web.Controllers;

[Route("[controller]/{action=index}")]
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
    [Route("/categoria/listar/despesas")]
    public async Task<IActionResult> ListarCategoriasDespesas()
    {
        var ret = await _categoriaService.ListarCategoriasUsuario(TipoLancamento.Despesa);
        return Ok(ret);
    }

    [HttpGet()]
    [Route("/categoria/listar/receitas")]
    public async Task<IActionResult> ListarCategoriasReceitas()
    {
        var ret = await _categoriaService.ListarCategoriasUsuario(TipoLancamento.Receita);
        return Ok(ret);
    }

    #region Categoria Crud

    [HttpGet()]
    [Route("/categoria/obter/{id}")]
    public async Task<IActionResult> Obter(int id)
    {
        var ret = await _categoriaService.ObterCategoria(id);
        return Ok(ret);
    }

    [HttpPost()]
    [Route("/categoria/inserir")]
    public async Task<IActionResult> Inserir([FromBody] CategoriaCadastroDto categoria)
    {
        var ret = await _categoriaService.InserirCategoria(categoria);
        return Ok(ret);
    }

    [HttpPost()]
    [Route("/categoria/alterar")]
    public async Task<IActionResult> Alterar([FromBody] CategoriaCadastroDto categoria)
    {
        var ret = await _categoriaService.AlterarCategoria(categoria);
        return Ok(ret);
    }

    [HttpPost()]
    [Route("/categoria/excluir")]
    public async Task<IActionResult> Excluir([FromBody] int id)
    {
        var ret = await _categoriaService.ExcluirCategoria(id);
        return Ok(ret);
    }

    #endregion

    #region Subcategoria Crud

    [HttpGet()]
    [Route("/categoria/sub/obter/{id}")]
    public async Task<IActionResult> ObterSubcategoria(int id)
    {
        var ret = await _categoriaService.ObterSubcategoria(id);
        return Ok(ret);
    }

    [HttpPost()]
    [Route("/categoria/sub/inserir")]
    public async Task<IActionResult> InserirSubcategoria([FromBody] SubcategoriaCadastroDto sub)
    {
        var ret = await _categoriaService.InserirSubcategoria(sub);
        return Ok(ret);
    }

    [HttpPost()]
    [Route("/categoria/sub/alterar")]
    public async Task<IActionResult> AlterarSubcategoria([FromBody] SubcategoriaCadastroDto sub)
    {
        var ret = await _categoriaService.AlterarSubcategoria(sub);
        return Ok(ret);
    }

    [HttpPost()]
    [Route("/categoria/sub/excluir")]
    public async Task<IActionResult> ExcluirSubcategoria([FromBody] int id)
    {
        var ret = await _categoriaService.ExcluirSubcategoria(id);
        return Ok(ret);
    }

    #endregion
}
