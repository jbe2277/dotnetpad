using System.Waf.Applications;
using System.Windows.Input;
using Waf.DotNetPad.Domain;

namespace Waf.DotNetPad.Applications.Services;

internal sealed class FileService : Model, IFileService
{
    private readonly ObservableList<DocumentFile> documentFiles;
    private DocumentFile? activeDocumentFile;
    private DocumentFile? lockedDocumentFile;
    private ICommand newCSharpCommand = DelegateCommand.DisabledCommand;
    private ICommand newVisualBasicCommand = DelegateCommand.DisabledCommand;
    private DelegateCommand newCSharpFromClipboardCommand = DelegateCommand.DisabledCommand;
    private DelegateCommand newVisualBasicFromClipboardCommand = DelegateCommand.DisabledCommand;
    private ICommand openCommand = DelegateCommand.DisabledCommand;
    private ICommand closeCommand = DelegateCommand.DisabledCommand;
    private ICommand closeAllCommand = DelegateCommand.DisabledCommand;
    private ICommand saveCommand = DelegateCommand.DisabledCommand;
    private ICommand saveAsCommand = DelegateCommand.DisabledCommand;

    public FileService()
    {
        documentFiles = [];
        DocumentFiles = new ReadOnlyObservableList<DocumentFile>(documentFiles);
    }

    public IReadOnlyObservableList<DocumentFile> DocumentFiles { get; }

    public DocumentFile? ActiveDocumentFile
    {
        get => activeDocumentFile;
        set
        {
            if (activeDocumentFile == value) return;
            if (value != null && !documentFiles.Contains(value)) throw new ArgumentException("value is not an item of the Documents collection.");
            activeDocumentFile = value;
            RaisePropertyChanged();
        }
    }

    public DocumentFile? LockedDocumentFile
    {
        get => lockedDocumentFile;
        set => SetProperty(ref lockedDocumentFile, value);
    }

    public ICommand NewCSharpCommand
    {
        get => newCSharpCommand;
        set => SetProperty(ref newCSharpCommand, value);
    }

    public ICommand NewVisualBasicCommand
    {
        get => newVisualBasicCommand;
        set => SetProperty(ref newVisualBasicCommand, value);
    }

    public DelegateCommand NewCSharpFromClipboardCommand
    {
        get => newCSharpFromClipboardCommand;
        set => SetProperty(ref newCSharpFromClipboardCommand, value);
    }

    public DelegateCommand NewVisualBasicFromClipboardCommand
    {
        get => newVisualBasicFromClipboardCommand;
        set => SetProperty(ref newVisualBasicFromClipboardCommand, value);
    }

    public ICommand OpenCommand
    {
        get => openCommand;
        set => SetProperty(ref openCommand, value);
    }

    public ICommand CloseCommand
    {
        get => closeCommand;
        set => SetProperty(ref closeCommand, value);
    }

    public ICommand CloseAllCommand
    {
        get => closeAllCommand;
        set => SetProperty(ref closeAllCommand, value);
    }

    public ICommand SaveCommand
    {
        get => saveCommand;
        set => SetProperty(ref saveCommand, value);
    }

    public ICommand SaveAsCommand
    {
        get => saveAsCommand;
        set => SetProperty(ref saveAsCommand, value);
    }

    public void AddDocument(DocumentFile document) => documentFiles.Add(document);

    public void RemoveDocument(DocumentFile document) => documentFiles.Remove(document);

    public void ClearDocuments() => documentFiles.Clear();
}
