using Microsoft.AspNetCore.Mvc;
using MoneyLoris.Web.Controllers.Base;

namespace MoneyLoris.Web.Controllers;

[Route("[controller]/{action=index}")]
public class RelatorioController : BaseController
{
    public IActionResult Index()
    {
        return View("~/Views/Relatorio/LancamentoCategoria/LancamentoCategoriaIndex.cshtml");
    }
}
