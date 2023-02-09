using Microsoft.AspNetCore.Mvc;

namespace MoneyLoris.Web.Controllers;
public class LancamentoController : Controller
{
    public IActionResult Index()
    {
        return View("LancamentoIndex");
    }
}
