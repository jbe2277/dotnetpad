using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Waf.DotNetPad.Presentation.Controls;

public class CodeCompletionDescription : Model
{
    public CodeCompletionDescription(Task<ImmutableArray<TaggedText>> lazySummary)
    {
        UpdateSummary(lazySummary);
    }

    public ImmutableArray<TaggedText> Summary { get; private set => SetProperty(ref field, value); }

    private async void UpdateSummary(Task<ImmutableArray<TaggedText>> lazySummary)
    {
        Summary = await lazySummary;
    }
}
