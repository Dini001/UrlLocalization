namespace RubberDucking.UrlLocalization.RouteTransformation;

public ref struct RoutesSegments
{
    public required string Culture { get; set; }
    public required string? Area { get; set; }
    public required string? Controller { get; set; }
    public required string? Action { get; set; }
}
