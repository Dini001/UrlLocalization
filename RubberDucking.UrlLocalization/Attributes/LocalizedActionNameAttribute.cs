namespace RubberDucking.UrlLocalization.Attributes;

/// <summary>
/// The name for the action in the url
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class LocalizedActionNameAttribute : Attribute
{
    public required string Culture { get; init; }
    public string? Name { get; init; }
    public bool ExcludeActionName { get; init; }
}
