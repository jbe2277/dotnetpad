using System.Waf.Applications;
using System.Windows.Input;
using Waf.DotNetPad.Domain;

namespace Waf.DotNetPad.Applications.Services;

internal sealed class FileService : Model, IFileService
{
    private readonly ObservableList<DocumentFile> documentFiles = [];

    public FileService()
    {
        DocumentFiles = new ReadOnlyObservableList<DocumentFile>(documentFiles);
    }

    public IReadOnlyObservableList<DocumentFile> DocumentFiles { get; }

    public DocumentFile? ActiveDocumentFile
    {
        get;
        set
        {
            if (field == value) return;
            if (value != null && !documentFiles.Contains(value)) throw new ArgumentException("value is not an item of the Documents collection.");
            field = value;
            RaisePropertyChanged();
        }
    }

    public DocumentFile? LockedDocumentFile { get; set => SetProperty(ref field, value); }

    public ICommand NewCSharpCommand { get; set => SetProperty(ref field, value); } = DelegateCommand.DisabledCommand;

    public ICommand NewVisualBasicCommand { get; set => SetProperty(ref field, value); } = DelegateCommand.DisabledCommand;

    public DelegateCommand NewCSharpFromClipboardCommand { get; set => SetProperty(ref field, value); } = DelegateCommand.DisabledCommand;

    public DelegateCommand NewVisualBasicFromClipboardCommand { get; set => SetProperty(ref field, value); } = DelegateCommand.DisabledCommand;

    public ICommand OpenCommand { get; set => SetProperty(ref field, value); } = DelegateCommand.DisabledCommand;

    public ICommand CloseCommand { get; set => SetProperty(ref field, value); } = DelegateCommand.DisabledCommand;

    public ICommand CloseAllCommand { get; set => SetProperty(ref field, value); } = DelegateCommand.DisabledCommand;

    public ICommand SaveCommand { get; set => SetProperty(ref field, value); } = DelegateCommand.DisabledCommand;

    public ICommand SaveAsCommand { get; set => SetProperty(ref field, value); } = DelegateCommand.DisabledCommand;

    public void AddDocument(DocumentFile document) => documentFiles.Add(document);

    public void RemoveDocument(DocumentFile document) => documentFiles.Remove(document);

    public void ClearDocuments() => documentFiles.Clear();
}
