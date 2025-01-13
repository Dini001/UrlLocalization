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
        if (path.Length == 1)
            return null;
        var endSegment = path.IndexOf('/', 1); //The first char of the Path is always a '/'
        if (endSegment == -1)
            endSegment = path.IndexOf('?', 1);
        if (endSegment == -1)
            endSegment = path.Length;

        var cultureSegment = path.AsSpan()[1..endSegment];
        foreach (var culture in cultures)   //cultures.Contains(cultureSegment)
            if (cultureSegment.Equals(culture, StringComparison.OrdinalIgnoreCase))
                return culture;

        return null;
    }
}