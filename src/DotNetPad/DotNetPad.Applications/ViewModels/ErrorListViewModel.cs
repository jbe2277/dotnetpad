using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Windows.Input;
using Waf.DotNetPad.Applications.Services;
using Waf.DotNetPad.Applications.Views;
using Waf.DotNetPad.Domain;

namespace Waf.DotNetPad.Applications.ViewModels;

[Export]
public class ErrorListViewModel : ViewModel<IErrorListView>
{
    private readonly ICodeEditorService codeEditorService;
    private readonly IClipboardService clipboardService;
    private readonly DelegateCommand gotoErrorCommand;
    private readonly DelegateCommand copyErrorCommand;
    private ErrorListItem? selectedErrorListItem;

    [ImportingConstructor]
    public ErrorListViewModel(IErrorListView view, IDocumentService documentService, ICodeEditorService codeEditorService, IClipboardService clipboardService) : base(view)
    {
        DocumentService = documentService;
        this.codeEditorService = codeEditorService;
        this.clipboardService = clipboardService;
        gotoErrorCommand = new DelegateCommand(GotoError, CanGotoError);
        copyErrorCommand = new DelegateCommand(CopyError, CanCopyError);
    }

    public IDocumentService DocumentService { get; }

    public ICommand GotoErrorCommand => gotoErrorCommand;

    public ICommand CopyErrorCommand => copyErrorCommand;

    public ErrorListItem? SelectedErrorListItem
    {
        get => selectedErrorListItem;
        set
        {
            if (!SetProperty(ref selectedErrorListItem, value)) return;
            gotoErrorCommand.RaiseCanExecuteChanged();
            copyErrorCommand.RaiseCanExecuteChanged();
        }
    }

    private bool CanGotoError() => DocumentService.ActiveDocumentFile != null && SelectedErrorListItem != null;

    private void GotoError() => codeEditorService.SetCaret(DocumentService.ActiveDocumentFile!, SelectedErrorListItem!.StartLine, SelectedErrorListItem.StartColumn);

    private bool CanCopyError() => SelectedErrorListItem != null;

    private void CopyError() => clipboardService.SetText(SelectedErrorListItem!.Description);
}
