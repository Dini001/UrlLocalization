using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RubberDucking.UrlLocalization.RouteTransformation;
using RubberDucking.UrlLocalization.Settings;

namespace RubberDucking.UrlLocalization;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRouteLocalization(this IServiceCollection services, Func<IServiceProvider, UrlLocalizationSettings> urlLocalizationSettings)
        => services
            .Configure<RouteOptions>(options =>
            {
                options.ConstraintMap.Add(nameof(RoutesSegments.Culture), typeof(RouteCultureConstraint));
            })
            .AddSingleton<RouteValueTransformer>()
            .AddSingleton(urlLocalizationSettings)

            //Remplace the default `IUrlHelperFactory` to remplace the `EndpointRoutingUrlHelper` generating the url (including inside TagHelpers)
            .RemoveAll(typeof(IUrlHelperFactory))
            .AddSingleton<IUrlHelperFactory, LocalizedUrlHelperFactory>();
}
