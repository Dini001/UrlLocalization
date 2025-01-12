## Description

Allows the localization of urls.

Exemple
```
/Controller/Action
/fr-CA/Controller/Action
/en-CA/Controller/Action
```

** Areas localization is not supported at the moment

## Quick start

```c#
static readonly string[] supportedCultures = ["fr-CA", "en-CA"];    // Could also be only the language ie: ["fr", "en"]
                                                                    // Use the casing desired for the url
const string DEFAULT_CULTURE_ROUTE_NAME = "DefaultRoute";
const string OTHER_CULTURE_ROUTE_NAME = "LocalizedRoute";

// Validate that all action are annoted with a translation attribute
// Program being an assembly marker for the project with the controllers
ValidationAttributs.ValiderAttributs<Program>(supportedCultures);

var builder = WebApplication.CreateBuilder(args)
builder.Services
    // Initialize AspNetCore to have multiple culture
    .Configure<RequestLocalizationOptions>(options =>
    {
        options
            .SetDefaultCulture(supportedCultures[0])
            .AddSupportedCultures(supportedCultures)
            .AddSupportedUICultures(supportedCultures)
            .RequestCultureProviders = [
                // Select the language for the current request base on the url. The culture part of the url must be in the "supportedCultures" list
                new RouteSegmentRequestCultureProvider(supportedCultures) {
                    Options = options
                }
            ];
    })
    .AddRouteLocalization(sp => new UrlLocalizationSettings()
    {
        DefaultCulture = supportedCultures[0],
        OtherCultures = supportedCultures[1..],
        RouteName = (ref RoutesSegments rs) => rs.Culture == supportedCultures[0] ? DEFAULT_CULTURE_ROUTE_NAME : OTHER_CULTURE_ROUTE_NAME
    })
    .AddControllersWithViews();

var app = builder.Build()

app
    // Set AspNetCore to change culture
    .UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value)

    // If the url start with the default culture, redirect to the url without the culture segment
    .UseMiddleware<DefaultCultureRedirectMiddleware>(supportedCultures[0])

app.UseRouting();

// Used to resolve the requested url
app.MapDynamicControllerRoute<LocalisationRouteValueTransformer>(
    "{culture:Culture}/{controller=Home}/{action=Index}/{id?}");
app.MapDynamicControllerRoute<LocalisationRouteValueTransformer>(
    "/{controller=Home}/{action=Index}/{id?}");

// Used to generate the urls via IUrlHelper
app.MapControllerRoute(
    name: OTHER_CULTURE_ROUTE_NAME,
    pattern: "{culture:Culture}/{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: DEFAULT_CULTURE_ROUTE_NAME,
    pattern: "{controller=Home}/{action=Index}/{id?}");
```

```c#
[LocalizedControllerName(Culture = "en-CA, Name = "Home")]
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
```

## Changelog

1.0.0 - 2025-01-12
- Creation of the librairy