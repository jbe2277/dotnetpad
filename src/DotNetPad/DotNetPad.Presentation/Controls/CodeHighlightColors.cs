using ICSharpCode.AvalonEdit.Highlighting;
using Microsoft.CodeAnalysis.Classification;
using System.Windows.Media;

namespace Waf.DotNetPad.Presentation.Controls;

internal static class CodeHighlightColors
{
    private static readonly CachedHighlightingColor defaultHighlightingColor = new(Colors.Black);
    private static readonly CachedHighlightingColor typeHighlightingColor = new(Colors.Teal);
    private static readonly CachedHighlightingColor commentHighlightingColor = new(Colors.Green);
    private static readonly CachedHighlightingColor xmlCommentHighlightingColor = new(Colors.Gray);
    private static readonly CachedHighlightingColor keywordHighlightingColor = new(Colors.Blue);
    private static readonly CachedHighlightingColor preprocessorKeywordHighlightingColor = new(Colors.Gray);
    private static readonly CachedHighlightingColor stringHighlightingColor = new(Colors.Maroon);

    private static readonly Dictionary<string, CachedHighlightingColor> highlightingColorsMap = new()
    {
        [ClassificationTypeNames.ClassName] = typeHighlightingColor,
        [ClassificationTypeNames.StructName] = typeHighlightingColor,
        [ClassificationTypeNames.InterfaceName] = typeHighlightingColor,
        [ClassificationTypeNames.DelegateName] = typeHighlightingColor,
        [ClassificationTypeNames.EnumName] = typeHighlightingColor,
        [ClassificationTypeNames.ModuleName] = typeHighlightingColor,
        [ClassificationTypeNames.TypeParameterName] = typeHighlightingColor,
        [ClassificationTypeNames.Comment] = commentHighlightingColor,
        [ClassificationTypeNames.XmlDocCommentAttributeName] = xmlCommentHighlightingColor,
        [ClassificationTypeNames.XmlDocCommentAttributeQuotes] = xmlCommentHighlightingColor,
        [ClassificationTypeNames.XmlDocCommentAttributeValue] = xmlCommentHighlightingColor,
        [ClassificationTypeNames.XmlDocCommentCDataSection] = xmlCommentHighlightingColor,
        [ClassificationTypeNames.XmlDocCommentComment] = xmlCommentHighlightingColor,
        [ClassificationTypeNames.XmlDocCommentDelimiter] = xmlCommentHighlightingColor,
        [ClassificationTypeNames.XmlDocCommentEntityReference] = xmlCommentHighlightingColor,
        [ClassificationTypeNames.XmlDocCommentName] = xmlCommentHighlightingColor,
        [ClassificationTypeNames.XmlDocCommentProcessingInstruction] = xmlCommentHighlightingColor,
        [ClassificationTypeNames.XmlDocCommentText] = commentHighlightingColor,
        [ClassificationTypeNames.Keyword] = keywordHighlightingColor,
        [ClassificationTypeNames.PreprocessorKeyword] = preprocessorKeywordHighlightingColor,
        [ClassificationTypeNames.StringLiteral] = stringHighlightingColor,
        [ClassificationTypeNames.VerbatimStringLiteral] = stringHighlightingColor
    };

    public static HighlightingColor DefaultHighlightingColor => defaultHighlightingColor;

    public static Color GetColor(string classificationTypeName) => GetHighlightingColorCore(classificationTypeName).Color;

    public static HighlightingColor GetHighlightingColor(string classificationTypeName) => GetHighlightingColorCore(classificationTypeName);

    private static CachedHighlightingColor GetHighlightingColorCore(string classificationTypeName)
    {
        highlightingColorsMap.TryGetValue(classificationTypeName, out var color);
        return color ?? defaultHighlightingColor;
    }


    private sealed class CachedHighlightingColor : HighlightingColor
    {
        public CachedHighlightingColor(Color color)
        {
            Color = color;
            Foreground = new SimpleHighlightingBrush(color);
            Freeze();
        }

        public Color Color { get; }
    }
}
