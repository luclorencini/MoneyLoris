using Microsoft.AspNetCore.Mvc;
using MoneyLoris.Web.Controllers.Base;

namespace MoneyLoris.Web.Controllers;

public class LancamentoController : BaseController
{
    public IActionResult Index()
    {
        return View("LancamentoIndex");
    }
}
