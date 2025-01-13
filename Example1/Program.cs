using Example1;
using Microsoft.Extensions.Options;
using RubberDucking.UrlLocalization;
using RubberDucking.UrlLocalization.RouteTransformation;
using RubberDucking.UrlLocalization.Settings;
using RubberDucking.UrlLocalization.Validation;

const string DEFAULT_CULTURE_ROUTE_NAME = "DefaultRoute";
const string OTHER_CULTURE_ROUTE_NAME = "LocalizedRoute";

// Validate that all action are annoted with a translation attribute
// Program being an assembly marker for the project with the controllers
AttributesValidation.ValidateAttributes<Program>(Cultures.SupportedCultures);

var builder = WebApplication.CreateBuilder(args);

builder.Services
    // Initialize AspNetCore to have multiple culture
    .Configure<RequestLocalizationOptions>(options =>
    {
        options
            .SetDefaultCulture(Cultures.SupportedCultures[0])
            .AddSupportedCultures(Cultures.SupportedCultures)
            .AddSupportedUICultures(Cultures.SupportedCultures)
            .RequestCultureProviders = [
                // Select the language for the current request base on the url.
                // The culture part of the url must be in the "supportedCultures" list
                new RouteSegmentRequestCultureProvider(Cultures.SupportedCultures) {
                    Options = options
                }
            ];
    })
    .AddRouteLocalization(sp => new UrlLocalizationSettings()
    {
        DefaultCulture = Cultures.SupportedCultures[0],
        OtherCultures = Cultures.SupportedCultures[1..],
        // Select which mapped controller route to use for generating the url based on the current request culture
        RouteName = (ref RoutesSegments rs) => rs.Culture == Cultures.SupportedCultures[0] ? DEFAULT_CULTURE_ROUTE_NAME : OTHER_CULTURE_ROUTE_NAME
    })
    .AddControllersWithViews();

var app = builder.Build();

app
    // Set AspNetCore to change culture
    .UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value)

    // If the url start with the default culture, redirect to the url without the culture segment
    .UseMiddleware<DefaultCultureRedirectMiddleware>(Cultures.SupportedCultures[0]);

app.UseRouting();

// Used to resolve the requested url,
// the default value should be in the default culture
app.MapDynamicControllerRoute<RouteValueTransformer>(
    "{culture:Culture}/{controller=Accueil}/{action=Index}/{id?}");
app.MapDynamicControllerRoute<RouteValueTransformer>(
    "/{controller=Accueil}/{action=Index}/{id?}");

// Used to generate the urls via IUrlHelper
app.MapControllerRoute(

    name: OTHER_CULTURE_ROUTE_NAME,
    pattern: "{culture:Culture}/{controller=Accueil}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: DEFAULT_CULTURE_ROUTE_NAME,
    pattern: "{controller=Accueil}/{action=Index}/{id?}");

app.Run();