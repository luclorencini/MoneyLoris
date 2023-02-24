using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyLoris.Application.Business.Usuarios.Dtos;
using MoneyLoris.Application.Business.Usuarios.Interfaces;
using MoneyLoris.Web.Controllers.Base;

namespace MoneyLoris.Web.Controllers;

[Authorize(Policy = "Administrador")]
[Route("[controller]/{action=index}")]
public class UsuarioController : BaseController
{
    private readonly IUsuarioService _usuarioService;

    public UsuarioController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    public IActionResult Index()
    {
        return View("UsuarioIndex");
    }

    [HttpPost()]
    public async Task<IActionResult> Pesquisar([FromBody] UsuarioPesquisaDto filtro)
    {
        var lista = await _usuarioService.Pesquisar(filtro);
        return Ok(lista);
    }

    [HttpPost()]
    public async Task<IActionResult> Inserir([FromBody] UsuarioCriacaoInputDto dados)
    {
        var id = await _usuarioService.CriarUsuario(dados);
        return Ok(id);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Info(int id)
    {
        var dado = await _usuarioService.ObterUsuario(id);
        return Ok(dado);
    }

    [HttpPost()]
    public async Task<IActionResult> Alterar([FromBody] UsuarioAlteracaoDto dados)
    {
        var id = await _usuarioService.AlterarUsuario(dados);
        return Ok(id);
    }

    [HttpPost()]
    public async Task<IActionResult> RedefinirSenha([FromBody] int id)
    {
        var idr = await _usuarioService.MarcarParaRedefinirSenha(id);
        return Ok(idr);
    }

    [HttpPost()]
    public async Task<IActionResult> Inativar([FromBody] int id)
    {
        var idr = await _usuarioService.InativarUsuario(id);
        return Ok(idr);
    }

    [HttpPost()]
    public async Task<IActionResult> Reativar([FromBody] int id)
    {
        var idr = await _usuarioService.ReativarUsuario(id);
        return Ok(idr);
    }

    [HttpPost()]
    public async Task<IActionResult> Excluir([FromBody] int id)
    {
        var idr = await _usuarioService.ExcluirUsuario(id);
        return Ok(idr);
    }
}
