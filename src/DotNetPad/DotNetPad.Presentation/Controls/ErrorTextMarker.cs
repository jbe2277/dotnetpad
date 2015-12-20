using ICSharpCode.AvalonEdit.Document;
using System.Windows.Media;

namespace Waf.DotNetPad.Presentation.Controls
{
    public class ErrorTextMarker : TextSegment
    {
        private readonly string message;
        private readonly Color markerColor;
        

        public ErrorTextMarker(int startOffset, int length, string message, Color markerColor)
        {
            this.StartOffset = startOffset;
            this.Length = length;
            this.message = message;
            this.markerColor = markerColor;
        }


        public string Message { get { return message; } }

        public Color MarkerColor { get { return markerColor; } }
    }
}
