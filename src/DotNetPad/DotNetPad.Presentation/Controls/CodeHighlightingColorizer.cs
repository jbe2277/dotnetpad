using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;

namespace Waf.DotNetPad.Presentation.Controls;

internal sealed class CodeHighlightingColorizer(Func<Microsoft.CodeAnalysis.Document> getDocument) : HighlightingColorizer
{
    protected override IHighlighter CreateHighlighter(TextView textView, TextDocument document) => new CodeHighlighter(document, getDocument);
}
