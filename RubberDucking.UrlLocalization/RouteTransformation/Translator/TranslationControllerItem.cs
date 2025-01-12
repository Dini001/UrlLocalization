using System.Diagnostics;

namespace RubberDucking.UrlLocalization.RouteTransformation.Translator;

[DebuggerDisplay("{DebuggerDisplay(),nq}")]
internal sealed class TranslationControllerItem
{
    public required string Culture { get; init; }
    public required string Name { get; init; }
    public required string OriginalName { get; init; }
    public List<TranslationActionItem> TranslationActions { get; } = [];
    private string DebuggerDisplay()
        => $"{Culture}: {OriginalName} => {Name}, {TranslationActions.Count} actions";
}