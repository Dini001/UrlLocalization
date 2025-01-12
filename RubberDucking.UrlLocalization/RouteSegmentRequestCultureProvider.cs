using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;

namespace RubberDucking.UrlLocalization;

public sealed class RouteSegmentRequestCultureProvider(string[] cultures) : RequestCultureProvider
{
    private readonly string[] cultures = cultures;

    public override Task<ProviderCultureResult?> DetermineProviderCultureResult(HttpContext httpContext)
    {
        var culture = GetCultureFromPath(httpContext) ?? cultures[0];
        return Task.FromResult<ProviderCultureResult?>(new ProviderCultureResult(culture));
    }

    private string? GetCultureFromPath(HttpContext httpContext)
    {
        if (!httpContext.Request.Path.HasValue)
            return null;

        var path = httpContext.Request.Path.Value;
        var indexSlash = path.IndexOf('/', 1); //The first char of the Path is always a '/'
        if (indexSlash == -1)
            return null;

        var segmentLangue = path.AsSpan()[1..indexSlash];
        foreach (var culture in cultures)   //cultures.Contains(segmentLangue)
            if (segmentLangue.Equals(culture, StringComparison.OrdinalIgnoreCase))
                return culture;

        return null;
    }
}