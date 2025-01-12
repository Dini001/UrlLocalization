using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using RubberDucking.UrlLocalization.RouteTransformation;
using RubberDucking.UrlLocalization.Settings;

namespace RubberDucking.UrlLocalization;

public sealed class RouteCultureConstraint(UrlLocalizationSettings urlLocalizationSettings) : IRouteConstraint
{
    private readonly UrlLocalizationSettings urlLocalizationSettings = urlLocalizationSettings;

    public bool Match(HttpContext? httpContext, IRouter? route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
    {
        if (!values.ContainsKey(nameof(RoutesSegments.Culture)))
            return false;

        var langue = (string?)values[nameof(RoutesSegments.Culture)];

        if (langue == null)
            return false;
        if (langue == urlLocalizationSettings.DefaultCulture)
            return true;
        foreach (var autreLangue in urlLocalizationSettings.OtherCultures)
        {
            if (autreLangue.Contains(langue, StringComparison.InvariantCultureIgnoreCase))
                return true;
        }

        return false;
    }
}
