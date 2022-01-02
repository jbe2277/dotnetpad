using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace Waf.DotNetPad.Presentation.Converters
{
    public class WindowTitleConverter : IMultiValueConverter
    {
        public object Convert(object?[] values, Type? targetType, object? parameter, CultureInfo? culture)
        {
            var stringList = values.OfType<string>().Where(x => !string.IsNullOrEmpty(x)).ToArray();
            if (stringList.Length == 2)
            {
                stringList = new[] { Path.GetFileName(stringList[0]), stringList[1] };
            }
            return string.Join(" - ", stringList);
        }

        public object[] ConvertBack(object? value, Type?[] targetTypes, object? parameter, CultureInfo? culture) => throw new NotSupportedException();
    }
}
