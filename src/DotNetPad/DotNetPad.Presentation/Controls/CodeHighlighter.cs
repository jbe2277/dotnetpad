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
        private readonly List<VersionedHighlightedLine> cachedLines;
        private readonly BlockingCollection<VersionedHighlightedLine> queue;
        private readonly CancellationTokenSource cancellationTokenSource;


        public CodeHighlighter(IDocument document, Func<Document> getDocument)
        {
            synchronizationContext = SynchronizationContext.Current;

            Document = document;
            this.getDocument = getDocument;
            queue = new BlockingCollection<VersionedHighlightedLine>();
            cachedLines = new List<VersionedHighlightedLine>();
            cancellationTokenSource = new CancellationTokenSource();
            StartWorker(cancellationTokenSource.Token);
        }


        public IDocument Document { get; }

        public HighlightingColor DefaultTextColor => CodeHighlightColors.DefaultColor;


        public event HighlightingStateChangedEventHandler HighlightingStateChanged;


        public HighlightedLine HighlightLine(int lineNumber)
        {
            var documentLine = Document.GetLineByNumber(lineNumber);
            var currentVersion = Document.Version;
            VersionedHighlightedLine cachedLine = null;
            
            for (var i = 0; i < cachedLines.Count; i++)
            {
                var line = cachedLines[i];
                if (line.DocumentLine != documentLine)
                {
                    continue;
                }
                if (currentVersion == null || !currentVersion.BelongsToSameDocumentAs(line.Version))
                {
                    cachedLines.RemoveAt(i);
                }
                else
                {
                    cachedLine = line;
                }
            }

            if (cachedLine != null && currentVersion.CompareAge(cachedLine.Version) == 0 
                && cachedLine.DocumentLine.Length == documentLine.Length)
            {
                return cachedLine;
            }
            
            
            var newLine = new VersionedHighlightedLine(Document, documentLine, Document.Version, cachedLine);
            queue.Add(newLine);
            cachedLines.Add(newLine);
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
                    var currentVersion = Document.Version;
                    if (line.Version == null || !currentVersion.BelongsToSameDocumentAs(line.Version) || currentVersion.CompareAge(line.Version) != 0)
                    {
                        continue;
                    }

                    var documentLine = line.DocumentLine;

                    var spans = await GetClassifiedSpansAsync(documentLine, cancellationToken).ConfigureAwait(false);
                    foreach (var section in line.Sections.ToArray().OfType<HighlightedSection>())
                    {
                        line.Sections.Remove(section);
                    }
                    foreach (var classifiedSpan in spans)
                    {
                        if (IsOutsideLine(documentLine, classifiedSpan.TextSpan.Start, classifiedSpan.TextSpan.Length))
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
                    await Task.Delay(1);
                }
            }
            catch (OperationCanceledException) { }
            catch (ObjectDisposedException) { }
        }

        private void RaiseHighlightingStateChanged(int fromLineNumber, int toLineNumber)
        {
            synchronizationContext.Post(state => HighlightingStateChanged?.Invoke(fromLineNumber, toLineNumber), null);
        }

        private static bool IsOutsideLine(IDocumentLine documentLine, int offset, int length)
        {
            return offset < documentLine.Offset || offset + length > documentLine.EndOffset;
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
        

        private sealed class VersionedHighlightedLine : HighlightedLine
        {
            public VersionedHighlightedLine(IDocument document, IDocumentLine documentLine, ITextSourceVersion version, VersionedHighlightedLine oldVersion) 
                : base(document, documentLine)
            {
                Version = version;
                if (oldVersion != null)
                {
                    foreach (var oldSection in oldVersion.Sections.OfType<HighlightedSection>())
                    {
                        if (IsOutsideLine(documentLine, oldSection.Offset, oldSection.Length))
                        {
                            continue;
                        }
                        Sections.Add(new HighlightedSection
                        {
                            Color = oldSection.Color,
                            Offset = oldSection.Offset,
                            Length = oldSection.Length
                        });
                    }
                }
            }

            public ITextSourceVersion Version { get; }
        }
    }
}
