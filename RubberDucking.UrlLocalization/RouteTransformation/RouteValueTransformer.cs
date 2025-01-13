using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using RubberDucking.UrlLocalization.Settings;

namespace RubberDucking.UrlLocalization.RouteTransformation;

public class RouteValueTransformer(UrlLocalizationSettings urlLocalizationSettings) : DynamicRouteValueTransformer
{
    private readonly UrlLocalizationSettings urlLocalizationSettings = urlLocalizationSettings;

    public override ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext, RouteValueDictionary values)
    {
        if (!values.ContainsKey(nameof(RoutesSegments.Controller)))
            return ValueTask.FromResult(values);

        var routesSegments = new RoutesSegments()
        {
            Culture = urlLocalizationSettings.DefaultCulture,
            Area = (string?)values[nameof(RoutesSegments.Area)],
            Controller = (string?)values[nameof(RoutesSegments.Controller)],
            Action = (string?)values[nameof(RoutesSegments.Action)]
        };
        if (values.TryGetValue(nameof(RoutesSegments.Culture), out var valLangue) && valLangue != null)
            routesSegments.Culture = (string)valLangue;

        Translator.Translator.Resolve(ref routesSegments);
        if (values.ContainsKey(nameof(RoutesSegments.Area)))
            values[nameof(RoutesSegments.Area)] = routesSegments.Area;

        if (values.ContainsKey(nameof(RoutesSegments.Controller)))
            values[nameof(RoutesSegments.Controller)] = routesSegments.Controller;

        if (values.ContainsKey(nameof(RoutesSegments.Action)))
            values[nameof(RoutesSegments.Action)] = routesSegments.Action;

        return ValueTask.FromResult(values);
    }
}