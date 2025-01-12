using System.Diagnostics;

namespace RubberDucking.UrlLocalization.RouteTransformation.Translator;

[DebuggerDisplay("{DebuggerDisplay(),nq}")]
internal sealed class TranslationActionItem
{
    public required string Name { get; init; }
    public required string OriginalName { get; init; }
    public required bool ExcludeActionName { get; init; }
    private string DebuggerDisplay()
        => $"{OriginalName} => {(ExcludeActionName ? "*action omitted*" : Name)}";
}
