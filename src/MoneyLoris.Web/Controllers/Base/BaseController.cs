using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MoneyLoris.Web.Controllers.Base;

[Authorize]
public class BaseController : Controller
{
}
