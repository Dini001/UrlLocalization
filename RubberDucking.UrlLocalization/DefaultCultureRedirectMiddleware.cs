using Microsoft.AspNetCore.Http;

namespace RubberDucking.UrlLocalization;

/// <summary>
/// If the url start with the default culture, redirect to the url without the culture segment
/// </summary>
public sealed class DefaultCultureRedirectMiddleware(RequestDelegate next, string defaultCulture)
{
    private readonly RequestDelegate next = next;
    private readonly string segmentDefaultCulture = $"/{defaultCulture}/";
    public async Task InvokeAsync(HttpContext httpContext)
    {
        var path = httpContext.Request.Path;
        if (path.HasValue && path.Value.StartsWith(segmentDefaultCulture, StringComparison.InvariantCultureIgnoreCase))
            httpContext.Response.Redirect(path.Value[3..]);
        await next(httpContext);
    }
}