using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Indentation.CSharp;
using ICSharpCode.AvalonEdit.Search;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Xml;
using Waf.DotNetPad.Applications.Services;
using Waf.DotNetPad.Domain;

namespace Waf.DotNetPad.Presentation.Controls
{
    public class CodeEditor : TextEditor
    {
        public static readonly DependencyProperty WorkspaceServiceProperty =
            DependencyProperty.Register("WorkspaceService", typeof(IWorkspaceService), typeof(CodeEditor), new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty DocumentFileProperty =
            DependencyProperty.Register("DocumentFile", typeof(DocumentFile), typeof(CodeEditor), new FrameworkPropertyMetadata(null, DocumentFileChangedCallback));


        private readonly Task updateTextTask;
        private readonly ErrorTextMarkerService errorMarkerService;
        private CompletionWindow completionWindow;
        private CancellationTokenSource recommendationCancellation;
        

        public CodeEditor()
        {
            HighlightingManager.Instance.RegisterHighlighting("C#", new[] { ".cs" }, CreateCSharpHighlightingDefinition);
            TextArea.IndentationStrategy = new CSharpIndentationStrategy(Options);
            SearchPanel.Install(TextArea);
            recommendationCancellation = new CancellationTokenSource();
            updateTextTask = Task.FromResult((object)null);

            TextArea.TextEntering += TextAreaTextEntering;
            TextArea.TextEntered += TextAreaTextEntered;

            errorMarkerService = new ErrorTextMarkerService(this);
            IsVisibleChanged += IsVisibleChangedHandler;
        }

        
        public IWorkspaceService WorkspaceService
        {
            get { return (IWorkspaceService)GetValue(WorkspaceServiceProperty); }
            set { SetValue(WorkspaceServiceProperty, value); }
        }

        public DocumentFile DocumentFile
        {
            get { return (DocumentFile)GetValue(DocumentFileProperty); }
            set { SetValue(DocumentFileProperty, value); }
        }


        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);

            if (WorkspaceService != null && DocumentFile != null)
            {
                WorkspaceService.UpdateText(DocumentFile, Text);
            }
        }

        private void TextAreaTextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && completionWindow != null)
            {
                if (!IsAllowedLanguageLetter(e.Text[0]))
                {
                    completionWindow.CompletionList.RequestInsertion(e);
                }
            }
        }

        private async void TextAreaTextEntered(object sender, TextCompositionEventArgs e)
        {
            recommendationCancellation.Cancel();
            
            if (WorkspaceService == null || DocumentFile == null)
            {
                return;
            }

            recommendationCancellation = new CancellationTokenSource();
            var cancellationToken = recommendationCancellation.Token;
            try
            {
                await updateTextTask;   // Wait for a pending UpdateText before calling GetRecommendedSymbolsAsync.
                cancellationToken.ThrowIfCancellationRequested();

                if (completionWindow == null && (e.Text == "." || IsAllowedLanguageLetter(e.Text[0])))
                {
                    var position = CaretOffset;
                    var word = GetWord(position);

                    var symbols = await WorkspaceService.GetRecommendedSymbolsAsync(DocumentFile, position, cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();
                    var symbolGroups = symbols.GroupBy(x => x.Name).ToArray();
                    if (symbolGroups.Any())
                    {
                        using (new PerformanceTrace("CompletionWindow.Show", DocumentFile))
                        {
                            completionWindow = new CompletionWindow(TextArea)
                            {
                                WindowStyle = WindowStyle.None,
                                AllowsTransparency = true,
                                MaxWidth = completionWindow.Width = 340,
                                MaxHeight = completionWindow.Height = 206
                            };
                            foreach (var symbolGroup in symbolGroups)
                            {
                                completionWindow.CompletionList.CompletionData.Add(new CodeCompletionData(symbolGroup.Key, symbolGroup.ToArray()));
                            }

                            if (IsAllowedLanguageLetter(e.Text[0]))
                            {
                                completionWindow.StartOffset = word.Item1;
                                completionWindow.CompletionList.SelectItem(word.Item2);
                            }
                            completionWindow.Show();
                            completionWindow.Closed += (s2, e2) => completionWindow = null;
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        private Tuple<int, string> GetWord(int position)
        {
            var wordStart = TextUtilities.GetNextCaretPosition(TextArea.Document, position, LogicalDirection.Backward, CaretPositioningMode.WordStart);
            var text = TextArea.Document.GetText(wordStart, position - wordStart);
            return new Tuple<int, string>(wordStart, text);
        }

        private void UpdateErrorMarkers()
        {
            errorMarkerService.Clear();
            foreach (var errorListItem in DocumentFile.Content.ErrorList)
            {
                var startOffset = Document.GetOffset(new TextLocation(errorListItem.StartLine + 1, errorListItem.StartColumn + 1));
                var endOffset = Document.GetOffset(new TextLocation(errorListItem.EndLine + 1, errorListItem.EndColumn + 1));
                errorMarkerService.Create(startOffset, endOffset - startOffset, errorListItem.Description);
            }
        }
        
        private void DocumentContentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ErrorList")
            {
                UpdateErrorMarkers();
            }
        }

        private void IsVisibleChangedHandler(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!IsVisible) { recommendationCancellation.Cancel(); }
        }

        private static void DocumentFileChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = (CodeEditor)d;
            
            var oldDocumentFile = e.OldValue as DocumentFile;
            if (oldDocumentFile != null)
            {
                PropertyChangedEventManager.RemoveHandler(oldDocumentFile.Content, editor.DocumentContentPropertyChanged, "");
            }
            
            if (editor.DocumentFile?.Content != null)
            {
                var code = editor.DocumentFile.Content.Code;
                if (editor.Text != code)
                {
                    editor.Text = code;
                    editor.CaretOffset = editor.DocumentFile.StartCaretPosition;
                }

                PropertyChangedEventManager.AddHandler(editor.DocumentFile.Content, editor.DocumentContentPropertyChanged, "");
                editor.UpdateErrorMarkers();
            }
            else
            {
                editor.Text = "";
            }
        }

        private static bool IsAllowedLanguageLetter(char character)
        {
            return TextUtilities.GetCharacterClass(character) == CharacterClass.IdentifierPart;
        }

        private static IHighlightingDefinition CreateCSharpHighlightingDefinition()
        {
            using (Stream stream = Application.GetResourceStream(new Uri("/Resources/Highlighting/CSharp-Mode.xshd", UriKind.Relative)).Stream)
            {
                using (XmlReader reader = new XmlTextReader(stream))
                {
                    return HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
        }
    }
}
