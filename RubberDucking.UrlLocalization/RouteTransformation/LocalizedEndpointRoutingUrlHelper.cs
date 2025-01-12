using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using RubberDucking.UrlLocalization.Settings;

namespace RubberDucking.UrlLocalization.RouteTransformation;

/// <summary>
/// Replace the implementation of <see cref="EndpointRoutingUrlHelper"/> to have url without the culture in the url for the default
/// </summary>
sealed class LocalizedEndpointRoutingUrlHelper(ActionContext actionContext, UrlLocalizationSettings urlLocalizationSettings)
    : UrlHelperBase(actionContext), IUrlHelper
{
    private readonly UrlLocalizationSettings urlLocalizationSettings = urlLocalizationSettings;

    public override string? Action(UrlActionContext urlActionContext)
    {
        ArgumentNullException.ThrowIfNull(urlActionContext);

        var values = GetValuesDictionary(urlActionContext.Values);

        var routesSegments = new RoutesSegments()
        {
            Culture = ObtenirValeur(null, nameof(RoutesSegments.Culture)) ?? urlLocalizationSettings.DefaultCulture,
            Area = ObtenirValeur(null, nameof(RoutesSegments.Area)),
            Controller = ObtenirValeur(urlActionContext.Controller, nameof(RoutesSegments.Controller)),
            Action = ObtenirValeur(urlActionContext.Action, nameof(RoutesSegments.Action))
        };

        Translator.Translator.GetTranslation(ref routesSegments);
        if (routesSegments.Culture == urlLocalizationSettings.DefaultCulture)
            values.Remove(nameof(RoutesSegments.Culture));
        else
            values[nameof(RoutesSegments.Culture)] = routesSegments.Culture;
        values[nameof(RoutesSegments.Area)] = routesSegments.Area;
        values[nameof(RoutesSegments.Controller)] = routesSegments.Controller;
        values[nameof(RoutesSegments.Action)] = routesSegments.Action;

        var linkGenerator = ActionContext.HttpContext.RequestServices.GetRequiredService<LinkGenerator>();
        var path = linkGenerator.GetPathByRouteValues(
            ActionContext.HttpContext,
            routeName: urlLocalizationSettings.RouteName(ref routesSegments),
            values,
            fragment: urlActionContext.Fragment == null ? FragmentString.Empty : new FragmentString("#" + urlActionContext.Fragment));
        return GenerateUrl(urlActionContext.Protocol, urlActionContext.Host, path);

        string? ObtenirValeur(string? initialValue, string key)
        {
            if (initialValue != null)
                return initialValue;
            var value = values[key];
            if (value == null && AmbientValues.TryGetValue(key, out var val))
                value = val;
            return (string?)value;
        }
    }

    public override string? RouteUrl(UrlRouteContext routeContext)
        => throw new NotImplementedException("Should not be here; to adjust otherwise");
}
