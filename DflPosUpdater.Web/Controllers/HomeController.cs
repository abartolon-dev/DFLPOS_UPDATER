using Microsoft.AspNetCore.Mvc;

namespace DflPosUpdater.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Error()
    {
        return View("Error");
    }
}
