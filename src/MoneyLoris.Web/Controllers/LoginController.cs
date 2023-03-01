using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyLoris.Application.Business.Auth.Dtos;
using MoneyLoris.Application.Business.Auth.Interfaces;
using MoneyLoris.Web.Controllers.Base;

namespace MoneyLoris.Web.Controllers;

[AllowAnonymous]
public class LoginController : BaseController
{
    private readonly ILoginService _loginService;

    public LoginController(ILoginService loginService)
    {
        _loginService = loginService;
    }

    public IActionResult Index()
    {
        return View("LoginIndex");
    }

    [HttpPost("/Login/SignIn")]
    public async Task<IActionResult> Login([FromBody] LoginInputDto login)
    {
        var ret = await _loginService.Login(login);
        return Ok(ret);
    }

    [HttpPost("/Login/AlterarSenha")]
    public async Task<IActionResult> AlterarSenha([FromBody] AlteracaoSenhaDto dto)
    {
        var ret = await _loginService.AlterarSenha(dto);
        return Ok(ret);
    }

    [Route("/Logout")]
    public async Task<IActionResult> LogOut()
    {
        var ret = await _loginService.LogOut();

        if (ret.IsOk())
            return Redirect("/");
        else
            return Ok(ret);
    }
}
