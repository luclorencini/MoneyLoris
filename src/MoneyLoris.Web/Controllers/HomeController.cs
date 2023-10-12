using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MoneyLoris.Web.Controllers;

[AllowAnonymous]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return Redirect("/Lancamento");
    }

    // Futura tela pública. Por hora, redireciona pra tela de Lançamento (vai requerer login)

    [HttpGet("/ping")]
    public IActionResult Ping()
    {
        return Ok("Pong");
    }
}