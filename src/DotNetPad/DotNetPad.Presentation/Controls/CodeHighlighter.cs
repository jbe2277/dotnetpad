using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Waf.DotNetPad.Presentation.Controls
{
    public sealed class CodeHighlighter : IHighlighter
    {
        private readonly SynchronizationContext synchronizationContext;
        private readonly Func<Document> getDocument;
        private readonly List<CachedLine> cachedLines;
        private readonly BlockingCollection<HighlightedLine> queue;
        private readonly CancellationTokenSource cancellationTokenSource;


        public CodeHighlighter(IDocument document, Func<Document> getDocument)
        {
            synchronizationContext = SynchronizationContext.Current;

            Document = document;
            this.getDocument = getDocument;
            queue = new BlockingCollection<HighlightedLine>();
            cachedLines = new List<CachedLine>();
            cancellationTokenSource = new CancellationTokenSource();
            StartWorker(cancellationTokenSource.Token);
        }


        public IDocument Document { get; }

        public HighlightingColor DefaultTextColor => CodeHighlightColors.DefaultColor;


        public event HighlightingStateChangedEventHandler HighlightingStateChanged;


        public HighlightedLine HighlightLine(int lineNumber)
        {
            var documentLine = Document.GetLineByNumber(lineNumber);
            var newVersion = Document.Version;
            CachedLine cachedLine = null;
            
            for (var i = 0; i < cachedLines.Count; i++)
            {
                var line = cachedLines[i];
                if (line.DocumentLine != documentLine)
                {
                    continue;
                }
                if (newVersion == null || !newVersion.BelongsToSameDocumentAs(line.TextSourceVersion))
                {
                    cachedLines.RemoveAt(i);
                }
                else
                {
                    cachedLine = line;
                }
            }

            if (cachedLine != null && newVersion.CompareAge(cachedLine.TextSourceVersion) == 0 
                && cachedLine.DocumentLine.Length == documentLine.Length)
            {
                return cachedLine.HighlightedLine;
            }
            
            
            var newLine = new HighlightedLine(Document, documentLine);
            queue.Add(newLine);
            CacheLine(newLine);
            return newLine;
        }
        
        private async void StartWorker(CancellationToken cancellationToken)
        {
            await Dispatcher.CurrentDispatcher.InvokeAsync(() => { }, DispatcherPriority.Background);
            await Task.Run(() => Worker(cancellationToken), cancellationToken);
        }

        private async Task Worker(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var line = queue.Take(cancellationToken);
                    var documentLine = line.DocumentLine;

                    var spans = await GetClassifiedSpansAsync(documentLine, cancellationToken).ConfigureAwait(false);
                    foreach (var classifiedSpan in spans)
                    {
                        if (IsOutsideLine(classifiedSpan, documentLine))
                        {
                            continue;
                        }
                        line.Sections.Add(new HighlightedSection
                            {
                                Color = CodeHighlightColors.GetColor(classifiedSpan.ClassificationType),
                                Offset = classifiedSpan.TextSpan.Start,
                                Length = classifiedSpan.TextSpan.Length
                            });
                    }

                    RaiseHighlightingStateChanged(documentLine.LineNumber, documentLine.LineNumber);
                }
            }
            catch (OperationCanceledException) { }
            catch (ObjectDisposedException) { }
        }

        private void RaiseHighlightingStateChanged(int fromLineNumber, int toLineNumber)
        {
            synchronizationContext.Post(state => HighlightingStateChanged?.Invoke(fromLineNumber, toLineNumber), null);
        }

        private static bool IsOutsideLine(ClassifiedSpan classifiedSpan, IDocumentLine documentLine)
        {
            return classifiedSpan.TextSpan.Start < documentLine.Offset 
                || classifiedSpan.TextSpan.Start > documentLine.EndOffset 
                || classifiedSpan.TextSpan.End > documentLine.EndOffset;
        }

        private void CacheLine(HighlightedLine line)
        {
            if (Document.Version != null)
            {
                cachedLines.Add(new CachedLine(line, Document.Version));
            }
        }

        private async Task<IEnumerable<ClassifiedSpan>> GetClassifiedSpansAsync(IDocumentLine documentLine, CancellationToken cancellationToken)
        {
            var document = getDocument();
            var text = await document.GetTextAsync().ConfigureAwait(false);
            if (text.Length >= documentLine.Offset + documentLine.TotalLength)
            {
                return await Classifier.GetClassifiedSpansAsync(document, 
                    new TextSpan(documentLine.Offset, documentLine.TotalLength), cancellationToken).ConfigureAwait(false);
            }
            return Enumerable.Empty<ClassifiedSpan>();
        }
        
        public void BeginHighlighting()
        {
        }

        public void EndHighlighting()
        {
        }

        public HighlightingColor GetNamedColor(string name)
        {
            return null;
        }

        public IEnumerable<HighlightingColor> GetColorStack(int lineNumber)
        {
            return null;
        }

        public void UpdateHighlightingState(int lineNumber)
        {
        }

        public void Dispose()
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
            queue.Dispose();
            cachedLines.Clear();
        }
        

        private class CachedLine
        {
            public CachedLine(HighlightedLine highlightedLine, ITextSourceVersion textSourceVersion)
            {
                HighlightedLine = highlightedLine;
                TextSourceVersion = textSourceVersion;
            }

            public HighlightedLine HighlightedLine { get; }

            public ITextSourceVersion TextSourceVersion { get; }

            public IDocumentLine DocumentLine => HighlightedLine.DocumentLine;
        }
    }
}
