using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Waf.DotNetPad.Domain;

namespace Waf.DotNetPad.Presentation.Converters;

public class ErrorSeverityToImageConverter : IValueConverter
{
    public object Convert(object? value, Type? targetType, object? parameter, CultureInfo? culture)
    {
        var errorSeverity = (ErrorSeverity)value!;
        string resourceKey = errorSeverity switch
        {
            ErrorSeverity.Error => "ErrorImage",
            ErrorSeverity.Warning => "WarningImage",
            _ => "InfoImage",
        };
        return Application.Current.Resources[resourceKey];
    }

    public object ConvertBack(object? value, Type? targetType, object? parameter, CultureInfo? culture) => throw new NotSupportedException();
}
