using Microsoft.CodeAnalysis;
using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using Waf.DotNetPad.Applications.CodeAnalysis;
using Waf.DotNetPad.Presentation.Controls;

namespace Waf.DotNetPad.Presentation.Converters
{
    public class TaggedTextListConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var list = (ImmutableArray<TaggedText>)value;
            if (list.IsDefault)
            {
                return "...";
            }
            return CreateTextBlock(list);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        private static TextBlock CreateTextBlock(ImmutableArray<TaggedText> text)
        {
            var textBlock = new TextBlock() { MaxWidth = 600, TextWrapping = TextWrapping.Wrap };
            foreach (var part in text)
            {
                textBlock.Inlines.Add(CreateRun(part));
            }
            return textBlock;
        }

        private static Run CreateRun(TaggedText text)
        {
            var run = new Run(text.ToString());
            var classificationTypeName = ClassificationTags.GetClassificationTypeName(text.Tag);
            run.Foreground = new SolidColorBrush(CodeHighlightColors.GetColor(classificationTypeName));
            return run;
        }
    }
}
