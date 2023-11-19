using System.Globalization;
using System.Text;

namespace Waf.DotNetPad.Applications.Host;

public class DelegateTextWriter(Action<string?> appendTextAction) : TextWriter(CultureInfo.CurrentCulture)
{
    public override Encoding Encoding => Encoding.UTF8;
        
    public override void Write(char value) => appendTextAction(value.ToString(CultureInfo.CurrentCulture));

    public override void Write(char[] buffer, int index, int count)
    {
        if (index != 0 || count != buffer.Length) buffer = buffer.Skip(index).Take(count).ToArray();
        appendTextAction(new string(buffer));
    }

    public override void Write(string? value) => appendTextAction(value);
}
