using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MoneyLoris.Web.Controllers;

[AllowAnonymous]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet("/app")]
    public IActionResult App()
    {
        return Redirect("/Lancamento");
    }

    [HttpGet("/ping")]
    public IActionResult Ping()
    {
        return Ok("Pong");
    }
}