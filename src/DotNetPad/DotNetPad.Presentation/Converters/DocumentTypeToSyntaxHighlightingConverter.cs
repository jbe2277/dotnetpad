using ICSharpCode.AvalonEdit.Highlighting;
using System;
using System.Globalization;
using System.Windows.Data;
using Waf.DotNetPad.Domain;

namespace Waf.DotNetPad.Presentation.Converters
{
    public class DocumentTypeToSyntaxHighlightingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var documentType = (DocumentType)value;
            if (documentType == DocumentType.CSharp) 
            {
                return HighlightingManager.Instance.GetDefinition("C#");
            }
            else
            {
                return HighlightingManager.Instance.GetDefinition("VB");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
