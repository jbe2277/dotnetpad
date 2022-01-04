using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Waf.DotNetPad.Presentation.Controls;

public class ErrorTextMarkerService : IBackgroundRenderer, IVisualLineTransformer
{
    private readonly TextEditor textEditor;
    private readonly TextSegmentCollection<ErrorTextMarker> markers;
    private ToolTip? toolTip;

    public ErrorTextMarkerService(TextEditor textEditor)
    {
        this.textEditor = textEditor;
        markers = new TextSegmentCollection<ErrorTextMarker>(textEditor.Document);

        TextView textView = textEditor.TextArea.TextView;
        textView.BackgroundRenderers.Add(this);
        textView.LineTransformers.Add(this);
        textView.Services.AddService(typeof(ErrorTextMarkerService), this);

        textView.MouseHover += TextViewMouseHover;
        textView.MouseHoverStopped += TextViewMouseHoverStopped;
        textView.VisualLinesChanged += TextViewVisualLinesChanged;
    }

    public KnownLayer Layer => KnownLayer.Selection;

    public void Create(int offset, int length, string message)
    {
        var marker = new ErrorTextMarker(offset, length, message, Colors.Red);
        markers.Add(marker);
        textEditor.TextArea.TextView.Redraw(marker);
    }

    public void Clear()
    {
        var oldMarkers = markers.ToArray();
        markers.Clear();
        foreach (var x in oldMarkers) textEditor.TextArea.TextView.Redraw(x);
    }

    public void Draw(TextView textView, DrawingContext drawingContext)
    {
        if (!markers.Any() || !textView.VisualLinesValid) return;
        var visualLines = textView.VisualLines;
        if (visualLines.Count == 0) return;
            
        int viewStart = visualLines.First().FirstDocumentLine.Offset;
        int viewEnd = visualLines.Last().LastDocumentLine.EndOffset;
        foreach (var marker in markers.FindOverlappingSegments(viewStart, viewEnd - viewStart))
        {
            foreach (var rect in BackgroundGeometryBuilder.GetRectsForSegment(textView, marker))
            {
                var startPoint = rect.BottomLeft;
                var endPoint = rect.BottomRight;

                var pen = new Pen(new SolidColorBrush(marker.MarkerColor), 1);
                pen.Freeze();
                    
                const double offset = 2.5;
                int count = Math.Max((int)((endPoint.X - startPoint.X) / offset) + 1, 4);

                var geometry = new StreamGeometry();
                using (var ctx = geometry.Open())
                {
                    ctx.BeginFigure(startPoint, false, false);
                    ctx.PolyLineTo(CreatePoints(startPoint, offset, count).ToArray(), true, false);
                }
                geometry.Freeze();

                drawingContext.DrawGeometry(Brushes.Transparent, pen, geometry);
                break;
            }
        }
    }

    public void Transform(ITextRunConstructionContext context, IList<VisualLineElement> elements) { }

    private void TextViewMouseHover(object sender, MouseEventArgs e)
    {
        if (!markers.Any()) return;
            
        var position = textEditor.TextArea.TextView.GetPositionFloor(e.GetPosition(textEditor.TextArea.TextView) + textEditor.TextArea.TextView.ScrollOffset);
        if (position.HasValue)
        {
            var logicalPosition = position.Value.Location;
            int offset = textEditor.Document.GetOffset(logicalPosition);

            var markersAtOffset = markers.FindSegmentsContaining(offset);
            var marker = markersAtOffset.LastOrDefault(m => !string.IsNullOrEmpty(m.Message));

            if (marker != null)
            {
                if (toolTip == null)
                {
                    toolTip = new ToolTip();
                    toolTip.Closed += (s2, e2) => toolTip = null;
                    toolTip.PlacementTarget = textEditor;
                    toolTip.Content = new TextBlock { Text = marker.Message, TextWrapping = TextWrapping.Wrap };
                    toolTip.IsOpen = true;
                    e.Handled = true;
                }
            }
        }
    }

    private void TextViewMouseHoverStopped(object sender, MouseEventArgs e)
    {
        if (toolTip != null)
        {
            toolTip.IsOpen = false;
            e.Handled = true;
        }
    }

    private void TextViewVisualLinesChanged(object? sender, EventArgs e)
    {
        if (toolTip != null) toolTip.IsOpen = false;
    }

    private static IEnumerable<Point> CreatePoints(Point start, double offset, int count)
    {
        for (int i = 0; i < count; i++) yield return new Point(start.X + (i * offset), start.Y - ((i + 1) % 2 == 0 ? offset : 0));
    }
}
