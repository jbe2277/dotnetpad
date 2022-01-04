namespace Waf.DotNetPad.Applications.Services;

public interface IClipboardService
{
    bool ContainsText();

    string GetText();

    void SetText(string text);
}
