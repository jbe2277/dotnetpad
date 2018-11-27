using ICSharpCode.AvalonEdit.Document;
using System.Windows.Media;

namespace Waf.DotNetPad.Presentation.Controls
{
    public class ErrorTextMarker : TextSegment
    {
        public ErrorTextMarker(int startOffset, int length, string message, Color markerColor)
        {
            StartOffset = startOffset;
            Length = length;
            Message = message;
            MarkerColor = markerColor;
        }

        public string Message { get; }

        public Color MarkerColor { get; }
    }
}
