using RubberDucking.UrlLocalization.RouteTransformation;

namespace RubberDucking.UrlLocalization.Settings;

public sealed class UrlLocalizationSettings
{
    public required string DefaultCulture { get; init; }
    public required string[] OtherCultures { get; init; }
    public required GetRouteName RouteName { get; init; }
    public delegate string? GetRouteName(ref RoutesSegments routesSegments);
}