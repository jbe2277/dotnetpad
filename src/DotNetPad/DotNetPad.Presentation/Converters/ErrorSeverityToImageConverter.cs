using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Waf.DotNetPad.Domain;

namespace Waf.DotNetPad.Presentation.Converters
{
    public class ErrorSeverityToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var errorSeverity = (ErrorSeverity)value;
            string resourceKey;
            if (errorSeverity == ErrorSeverity.Error) { resourceKey = "ErrorImage"; }
            else if (errorSeverity == ErrorSeverity.Warning) { resourceKey = "WarningImage"; }
            else { resourceKey = "InfoImage"; }

            return Application.Current.Resources[resourceKey];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
