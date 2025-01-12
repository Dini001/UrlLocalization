using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using RubberDucking.UrlLocalization.Settings;
using System.Diagnostics;

namespace RubberDucking.UrlLocalization.RouteTransformation;

/// <summary>
/// Replace the implementation of <see cref="UrlHelperFactory"/>.
/// Does the same validation but return an instance of <see cref="LocalizedEndpointRoutingUrlHelper"/> instead of <see cref="EndpointRoutingUrlHelper"/>.
/// </summary>
public sealed class LocalizedUrlHelperFactory : IUrlHelperFactory
{
    public IUrlHelper GetUrlHelper(ActionContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var httpContext = context.HttpContext
                            ?? throw new ArgumentException($"The '{nameof(ActionContext.HttpContext)}' property of '{nameof(ActionContext)}' must not be null.");
        if (httpContext.Items == null)
            throw new ArgumentException($"The '{nameof(HttpContext.Items)}' property of '{nameof(HttpContext)}' must not be null.");

        // Perf: Create only one UrlHelper per context
        if (httpContext.Items.TryGetValue(typeof(IUrlHelper), out var value) && value is IUrlHelper urlHelper)
            return urlHelper;

        var endpoint = httpContext.GetEndpoint();
        if (endpoint != null)
        {
            var serviceProvider = context.HttpContext.RequestServices;
            urlHelper = new LocalizedEndpointRoutingUrlHelper(context,
                                                              serviceProvider.GetRequiredService<UrlLocalizationSettings>());
        }
        else
            //urlHelper = new UrlHelper(context);
            throw new UnreachableException("Should not be here; to adjust otherwise");

        httpContext.Items[typeof(IUrlHelper)] = urlHelper;

        return urlHelper;
    }
}
