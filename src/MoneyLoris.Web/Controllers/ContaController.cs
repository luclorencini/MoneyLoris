using Microsoft.AspNetCore.Mvc;
using MoneyLoris.Web.Controllers.Base;

namespace MoneyLoris.Web.Controllers;

public class ContaController : BaseController
{
    public IActionResult Index()
    {
        return View("ContaIndex");
    }
}
