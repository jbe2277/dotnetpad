using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;

namespace Waf.DotNetPad.Presentation.Converters;

public class TabFileNameConverter : IMultiValueConverter
{
    private const int MaxCharacters = 40;

    public object Convert(object?[] values, Type? targetType, object? parameter, CultureInfo? culture)
    {
        if (values == null || values.Length != 2 || values[0] is not string v0 || values[1] is not bool modified) return DependencyProperty.UnsetValue;
            
        var fileName = Path.GetFileName(v0);
        if (fileName.Length > MaxCharacters) fileName = fileName.Remove(MaxCharacters - 3) + "...";
        return fileName + (modified ? "*" : "");
    }

    public object[] ConvertBack(object? value, Type?[] targetTypes, object? parameter, CultureInfo? culture) => throw new NotSupportedException();
}
