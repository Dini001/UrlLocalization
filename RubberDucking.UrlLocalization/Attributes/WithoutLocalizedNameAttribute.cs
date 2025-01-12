namespace RubberDucking.UrlLocalization.Attributes;

/// <summary>
/// For the url that do not need to be localized
/// The attribute is used for validation, but not during runtime
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public sealed class WithoutLocalizedNameAttribute : Attribute
{ }
