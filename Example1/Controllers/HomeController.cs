using Microsoft.AspNetCore.Mvc;
using RubberDucking.UrlLocalization.Attributes;
namespace Example1.Controllers;

[LocalizedControllerName(Culture = "en-CA", Name = "Home")]
[LocalizedControllerName(Culture = "fr-CA", Name = "Accueil")]
public class HomeController : Controller
{
    [HttpGet]
    [LocalizedActionName(Culture = "fr-CA", ExcludeActionName = true)]
    [LocalizedActionName(Culture = "en-CA", ExcludeActionName = true)]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    [LocalizedActionName(Culture = "fr-CA", Name = "Modifier")]
    [LocalizedActionName(Culture = "en-CA", Name = "Edit")]
    public IActionResult Edit()
    {
        return View();
    }

    [WithoutLocalizedName]
    public IActionResult UrlNotSeemByUser()
    {
        return View();
    }
}
