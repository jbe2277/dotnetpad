using ICSharpCode.AvalonEdit.Document;
using System;
using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Windows;
using Waf.DotNetPad.Applications.Services;
using Waf.DotNetPad.Applications.ViewModels;
using Waf.DotNetPad.Applications.Views;

namespace Waf.DotNetPad.Presentation.Views
{
    [Export(typeof(ICodeEditorView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class CodeEditorView : ICodeEditorView
    {
        private readonly Lazy<CodeEditorViewModel> viewModel;
        

        public CodeEditorView()
        {
            InitializeComponent();
            viewModel = new Lazy<CodeEditorViewModel>(() => this.GetViewModel<CodeEditorViewModel>());
            Loaded += FirstTimeLoadedHandler;
            Loaded += LoadedHandler;
            Unloaded += UnloadedHandler;
            IsVisibleChanged += IsVisibleChangedHandler;
            codeEditor.TextArea.Caret.PositionChanged += CaretPositionChanged;
        }


        private CodeEditorViewModel ViewModel => viewModel.Value;


        private void FirstTimeLoadedHandler(object sender, RoutedEventArgs e)
        {
            Loaded -= FirstTimeLoadedHandler;
            codeEditor.Focus();
        }

        private async void LoadedHandler(object sender, RoutedEventArgs e)
        {
            // Delay the access of the ViewModel because otherwise it is not yet set as DataContext.
            await Dispatcher.InvokeAsync(() => { });
            ViewModel.CodeEditorService.RequestSetCaret += CodeEditorServiceRequestSetCaret;
        }

        private void UnloadedHandler(object sender, RoutedEventArgs e)
        {
            ViewModel.CodeEditorService.RequestSetCaret -= CodeEditorServiceRequestSetCaret;
        }

        private void IsVisibleChangedHandler(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateCaretPosition();
        }

        private void CaretPositionChanged(object sender, EventArgs e)
        {
            UpdateCaretPosition();
        }

        private async void UpdateCaretPosition()
        {
            await Dispatcher.InvokeAsync(() => { });  // This is necessary so that the ViewModel can set the DataContext first.
            if (IsVisible)
            {
                ViewModel.ShellService.Line = codeEditor.TextArea.Caret.Line;
                ViewModel.ShellService.Column = codeEditor.TextArea.Caret.Column;
            }
        }

        private void CodeEditorServiceRequestSetCaret(object sender, SetCaretEventArgs e)
        {
            if (e.DocumentFile != ViewModel.DocumentFile) { return; }

            var offset = codeEditor.Document.GetOffset(new TextLocation(e.Line + 1, e.Column + 1));
            codeEditor.TextArea.Caret.Offset = offset;
            codeEditor.Focus();
        }
    }
}
