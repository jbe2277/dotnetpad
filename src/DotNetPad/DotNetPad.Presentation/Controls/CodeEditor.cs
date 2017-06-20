using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Search;
using Microsoft.CodeAnalysis.Completion;
using System;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using Microsoft.CodeAnalysis;
using Waf.DotNetPad.Applications.Services;
using Waf.DotNetPad.Domain;

namespace Waf.DotNetPad.Presentation.Controls
{
    public class CodeEditor : TextEditor
    {
        public static readonly DependencyProperty WorkspaceServiceProperty =
            DependencyProperty.Register(nameof(WorkspaceService), typeof(IWorkspaceService), typeof(CodeEditor), new FrameworkPropertyMetadata(null, WorkspaceServiceChangedCallback));

        public static readonly DependencyProperty DocumentFileProperty =
            DependencyProperty.Register(nameof(DocumentFile), typeof(DocumentFile), typeof(CodeEditor), new FrameworkPropertyMetadata(null, DocumentFileChangedCallback));


        private readonly ErrorTextMarkerService errorMarkerService;
        private CompletionWindow completionWindow;
        private CancellationTokenSource completionCancellation;
        private volatile IWorkspaceService workspaceService;
        private volatile DocumentFile documentFile;


        public CodeEditor()
        {
            SearchPanel.Install(TextArea);
            completionCancellation = new CancellationTokenSource();

            TextArea.TextView.LineTransformers.Insert(0, new CodeHighlightingColorizer(() => workspaceService.GetDocument(documentFile)));
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
            await ShowCompletionAsync(e.Text.FirstOrDefault());
        }

        protected override async void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == Key.Space && e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control)
                && !e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Shift))
            {
                e.Handled = true;
                await ShowCompletionAsync(null);
            }
        }

        private async Task ShowCompletionAsync(char? triggerChar)
        {
            completionCancellation.Cancel();

            if (WorkspaceService == null || DocumentFile == null)
            {
                return;
            }

            completionCancellation = new CancellationTokenSource();
            var cancellationToken = completionCancellation.Token;
            try
            {
                if (completionWindow == null && (triggerChar == null || triggerChar == '.' || IsAllowedLanguageLetter(triggerChar.Value)))
                {
                    var position = CaretOffset;
                    var word = GetWord(position);

                    var document = WorkspaceService.GetDocument(DocumentFile);
                    var completionService = CompletionService.GetService(document);

                    var completionList = await Task.Run(async () =>
                            await completionService.GetCompletionsAsync(document, position, cancellationToken: cancellationToken), cancellationToken);
                    if (completionList == null)
                    {
                        return;
                    }

                    cancellationToken.ThrowIfCancellationRequested();

                    using (new PerformanceTrace("CompletionWindow.Show", DocumentFile))
                    {
                        completionWindow = new CompletionWindow(TextArea)
                        {
                            WindowStyle = WindowStyle.None,
                            AllowsTransparency = true
                        };
                        completionWindow.MaxWidth = completionWindow.Width = 340;
                        completionWindow.MaxHeight = completionWindow.Height = 206;
                        foreach (var completionItem in completionList.Items)
                        {
                            completionWindow.CompletionList.CompletionData.Add(new CodeCompletionData(completionItem.DisplayText,
                                () => GetDescriptionAsync(completionService, document, completionItem), completionItem.Tags));
                        }

                        if (triggerChar == null || IsAllowedLanguageLetter(triggerChar.Value))
                        {
                            completionWindow.StartOffset = word.Item1;
                            completionWindow.CompletionList.SelectItem(word.Item2);
                        }
                        completionWindow.Show();
                        completionWindow.Closed += (s2, e2) => completionWindow = null;
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        private static async Task<ImmutableArray<TaggedText>> GetDescriptionAsync(CompletionService completionService, Document document, CompletionItem completionItem)
        {
            return (await Task.Run(async () => await completionService.GetDescriptionAsync(document, completionItem))).TaggedParts;
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
            if (e.PropertyName == nameof(DocumentContent.Code))
            {
                UpdateCode();
            }
            else if (e.PropertyName == nameof(DocumentContent.ErrorList))
            {
                UpdateErrorMarkers();
            }
        }

        private void IsVisibleChangedHandler(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!IsVisible) { completionCancellation.Cancel(); }
        }

        private bool UpdateCode()
        {
            var code = DocumentFile.Content.Code;
            if (Text != code)
            {
                Text = code;
                return true;
            }
            return false;
        }

        private static void WorkspaceServiceChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CodeEditor)d).workspaceService = (IWorkspaceService)e.NewValue;
        }

        private static void DocumentFileChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = (CodeEditor)d;
            editor.documentFile = (DocumentFile)e.NewValue;

            var oldDocumentFile = e.OldValue as DocumentFile;
            if (oldDocumentFile != null)
            {
                PropertyChangedEventManager.RemoveHandler(oldDocumentFile.Content, editor.DocumentContentPropertyChanged, "");
            }

            if (editor.DocumentFile?.Content != null)
            {
                if (editor.UpdateCode())
                {
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
    }
}
