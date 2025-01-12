namespace RubberDucking.UrlLocalization.Attributes;

/// <summary>
/// The name for the controller in the url
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class LocalizedControllerNameAttribute : Attribute
{
    public required string Culture { get; init; }
    public required string Name { get; init; }
}