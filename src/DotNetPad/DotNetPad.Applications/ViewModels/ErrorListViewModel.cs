using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Windows.Input;
using Waf.DotNetPad.Applications.Services;
using Waf.DotNetPad.Applications.Views;
using Waf.DotNetPad.Domain;

namespace Waf.DotNetPad.Applications.ViewModels
{
    [Export]
    public class ErrorListViewModel : ViewModel<IErrorListView>
    {
        private readonly IDocumentService documentService;
        private readonly ICodeEditorService codeEditorService;
        private readonly IClipboardService clipboardService;
        private readonly DelegateCommand gotoErrorCommand;
        private readonly DelegateCommand copyErrorCommand;
        private ErrorListItem selectedErrorListItem;

        
        [ImportingConstructor]
        public ErrorListViewModel(IErrorListView view, IDocumentService documentService, ICodeEditorService codeEditorService, IClipboardService clipboardService)
            : base(view)
        {
            this.documentService = documentService;
            this.codeEditorService = codeEditorService;
            this.clipboardService = clipboardService;
            this.gotoErrorCommand = new DelegateCommand(GotoError, CanGotoError);
            this.copyErrorCommand = new DelegateCommand(CopyError, CanCopyError);
        }


        public IDocumentService DocumentService { get { return documentService; } }

        public ICommand GotoErrorCommand { get { return gotoErrorCommand; } }

        public ICommand CopyErrorCommand { get { return copyErrorCommand; } }

        public ErrorListItem SelectedErrorListItem
        {
            get { return selectedErrorListItem; }
            set 
            { 
                if (SetProperty(ref selectedErrorListItem, value))
                {
                    gotoErrorCommand.RaiseCanExecuteChanged();
                    copyErrorCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private bool CanGotoError()
        {
            return SelectedErrorListItem != null;
        }

        private void GotoError()
        {
            codeEditorService.SetCaret(DocumentService.ActiveDocumentFile, SelectedErrorListItem.StartLine, SelectedErrorListItem.StartColumn);
        }

        private bool CanCopyError()
        {
            return SelectedErrorListItem != null;
        }

        private void CopyError()
        {
            clipboardService.SetText(SelectedErrorListItem.Description);
        }
    }
}
