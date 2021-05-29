using System.ComponentModel.Composition;
using System.Windows;
using Waf.DotNetPad.Applications.Services;

namespace Waf.DotNetPad.Presentation.Services
{
    [Export(typeof(IClipboardService))]
    internal class ClipboardService : IClipboardService
    {
        public bool ContainsText() => Clipboard.ContainsText();

        public string GetText() => Clipboard.GetText();

        public void SetText(string text) => Clipboard.SetText(text);
    }
}
