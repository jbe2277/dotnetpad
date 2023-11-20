using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Waf.Applications;
using System.Waf.Applications.Services;
using Waf.DotNetPad.Applications.CodeAnalysis;
using Waf.DotNetPad.Applications.Properties;
using Waf.DotNetPad.Applications.Services;
using Waf.DotNetPad.Applications.ViewModels;
using Waf.DotNetPad.Domain;

namespace Waf.DotNetPad.Applications.Controllers;

/// <summary>Responsible for the file related commands.</summary>
[Export]
internal sealed class FileController
{
    private static readonly string[] supportedFileExtensions = [".cs", ".vb"];
    private static readonly FileType cSharpFileType = new(Resources.CSharpFile, ".cs");
    private static readonly FileType visualBasicFileType = new(Resources.VisualBasicFile, ".vb");
    private static readonly FileType allFilesType = new(Resources.CodeFile, ".cs;*.vb");

    private readonly IMessageService messageService;
    private readonly IFileDialogService fileDialogService;
    private readonly IShellService shellService;
    private readonly IEnvironmentService environmentService;
    private readonly IClipboardService clipboardService;
    private readonly FileService fileService;
    private readonly ExportFactory<SaveChangesViewModel> saveChangesViewModelFactory;
    private readonly DelegateCommand closeCommand;
    private readonly DelegateCommand closeAllCommand;
    private readonly DelegateCommand saveCommand;
    private readonly DelegateCommand saveAsCommand;
    private DocumentFile? lastActiveDocumentFile;
    private IWeakEventProxy? activeDocumentPropertyChangedProxy;
    private int documentCounter;

    [ImportingConstructor]
    public FileController(IMessageService messageService, IFileDialogService fileDialogService, IShellService shellService, IEnvironmentService environmentService, 
        IClipboardService clipboardService, FileService fileService, ExportFactory<SaveChangesViewModel> saveChangesViewModelFactory)
    {
        this.messageService = messageService;
        this.fileDialogService = fileDialogService;
        this.shellService = shellService;
        this.environmentService = environmentService;
        this.clipboardService = clipboardService;
        this.fileService = fileService;
        this.saveChangesViewModelFactory = saveChangesViewModelFactory;
        closeCommand = new(CloseFile, CanCloseFile);
        closeAllCommand = new(CloseAll, CanCloseAll);
        saveCommand = new(SaveFile, CanSaveFile);
        saveAsCommand = new(SaveAsFile, CanSaveAsFile);

        this.fileService.NewCSharpCommand = new DelegateCommand(NewCSharpFile);
        this.fileService.NewVisualBasicCommand = new DelegateCommand(NewVisualBasicFile);
        this.fileService.NewCSharpFromClipboardCommand = new DelegateCommand(NewCSharpFromClipboard, CanNewFromClipboard);
        this.fileService.NewVisualBasicFromClipboardCommand = new DelegateCommand(NewVisualBasicFromClipboard, CanNewFromClipboard);
        this.fileService.OpenCommand = new DelegateCommand(OpenFile);
        this.fileService.CloseCommand = closeCommand;
        this.fileService.CloseAllCommand = closeAllCommand;
        this.fileService.SaveCommand = saveCommand;
        this.fileService.SaveAsCommand = saveAsCommand;

        WeakEvent.PropertyChanged.Add(fileService, FileServicePropertyChanged);
        shellService.Closing += ShellServiceClosing;
    }

    private DocumentFile? ActiveDocumentFile
    {
        get => fileService.ActiveDocumentFile;
        set => fileService.ActiveDocumentFile = value;
    }

    private DocumentFile? LockedDocumentFile => fileService.LockedDocumentFile;

    public void Initialize() => fileService.DocumentFiles.CollectionItemChanged += DocumentFilePropertyChanged;

    public async void Run()
    {
        await Task.Yield();  // Ensure that the ShellView is shown before loading files
        foreach (var fileToLoad in shellService.Settings.LastOpenedFiles)
        {
            OpenCore(fileToLoad, setActiveDocument: false);
        }
        foreach (var fileToLoad in environmentService.FilesToLoad)
        {
            OpenCore(fileToLoad);
        }
        if (!environmentService.FilesToLoad.Any()) NewCSharpFile();
    }

    private void NewCSharpFile(object? commandParameter = null) => NewCore(DocumentType.CSharp, TryGetCode(commandParameter));

    private void NewVisualBasicFile(object? commandParameter = null) => NewCore(DocumentType.VisualBasic, TryGetCode(commandParameter));

    private static string? TryGetCode(object? commandParameter) => commandParameter is Lazy<string> x ? x.Value : commandParameter as string;

    private bool CanNewFromClipboard() => clipboardService.ContainsText();

    private void NewCSharpFromClipboard()
    {
        string code = clipboardService.GetText();
        if (!string.IsNullOrEmpty(code)) NewCore(DocumentType.CSharp, code);
    }

    private void NewVisualBasicFromClipboard()
    {
        string code = clipboardService.GetText();
        if (!string.IsNullOrEmpty(code)) NewCore(DocumentType.VisualBasic, code);
    }

    private void OpenFile() => OpenCore();

    private bool CanCloseFile() => ActiveDocumentFile != null && ActiveDocumentFile != LockedDocumentFile;

    private void CloseFile() => CloseCore(ActiveDocumentFile!);

    private bool CanCloseAll() => ActiveDocumentFile != null && LockedDocumentFile == null;

    private bool CanSaveFile() => ActiveDocumentFile?.Modified == true;

    private void SaveFile() => Save(ActiveDocumentFile!);

    private bool CanSaveAsFile() => ActiveDocumentFile != null;

    private void SaveAsFile() => SaveAs(ActiveDocumentFile!);

    private void Save(DocumentFile document)
    {
        if (Path.IsPathRooted(document.FileName)) SaveCore(document, document.FileName);
        else SaveAs(document);
    }

    private void SaveAs(DocumentFile document)
    {
        var fileType = document.DocumentType == DocumentType.CSharp ? cSharpFileType : visualBasicFileType;
        var fileName = Path.GetFileNameWithoutExtension(document.FileName);
        var result = fileDialogService.ShowSaveFileDialog(shellService.ShellView, fileType, fileName);
        if (!result.IsValid) return;
        SaveCore(document, result.FileName!);
    }

    private void NewCore(DocumentType documentType, string? code = null)
    {
        int startCaretPosition = 0;
        if (string.IsNullOrEmpty(code))
        {
            code = documentType == DocumentType.CSharp ? TemplateCode.InitialCSharpCode : TemplateCode.InitialVisualBasicCode;
            startCaretPosition = documentType == DocumentType.CSharp ? TemplateCode.StartCaretPositionCSharp : TemplateCode.StartCaretPositionVisualBasic;
        }
        var fileName = "Script" + (documentCounter + 1) + (documentType == DocumentType.CSharp ? ".cs" : ".vb");
        var document = new DocumentFile(documentType, fileName, code!, startCaretPosition);
        document.ResetModified();
        fileService.AddDocument(document);
        ActiveDocumentFile = document;
        documentCounter++;
    }

    private bool PrepareToClose(IEnumerable<DocumentFile> documentsToClose)
    {
        var modifiedDocuments = documentsToClose.Where(d => d.Modified).ToArray();
        if (!modifiedDocuments.Any()) return true;

        var saveChangesViewModel = saveChangesViewModelFactory.CreateExport().Value;
        saveChangesViewModel.DocumentFiles = modifiedDocuments;
        bool? dialogResult = saveChangesViewModel.ShowDialog(shellService.ShellView);
        if (dialogResult == true)
        {
            foreach (var x in modifiedDocuments) Save(x);
        }
        return dialogResult != null;
    }

    private void CloseCore(DocumentFile document)
    {
        if (!PrepareToClose([document])) return;

        if (ActiveDocumentFile == document)
        {
            var nextDocument = fileService.DocumentFiles.GetNextElementOrDefault(ActiveDocumentFile) ?? fileService.DocumentFiles.LastOrDefault();
            ActiveDocumentFile = nextDocument;
        }
        fileService.RemoveDocument(document);
    }

    private void CloseAll()
    {
        if (!PrepareToClose(fileService.DocumentFiles)) return;
        ActiveDocumentFile = null;
        fileService.ClearDocuments();
    }
        
    private DocumentFile? OpenCore(string? fileName = null, bool setActiveDocument = true)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            var result = fileDialogService.ShowOpenFileDialog(shellService.ShellView, allFilesType);
            if (result.IsValid) fileName = result.FileName;
        }

        if (string.IsNullOrEmpty(fileName)) return null;
            
        // Check if document is already opened
        var document = fileService.DocumentFiles.SingleOrDefault(d => d.FileName == fileName);
        if (document == null)
        {
            string fileExtension = Path.GetExtension(fileName);
            if (!supportedFileExtensions.Contains(fileExtension))
            {
                Log.Default.Error("The extension of the file '{0}' is not supported.", fileName);
                messageService.ShowError(shellService.ShellView, Resources.OpenFileUnsupportedExtension, fileName);
                return null;
            }
            var documentType = cSharpFileType.FileExtensions.Contains(fileExtension) ? DocumentType.CSharp : DocumentType.VisualBasic;
            document = new DocumentFile(documentType, LoadDocumentContent) { FileName = fileName };
            fileService.AddDocument(document);
        }
        if (setActiveDocument) ActiveDocumentFile = document;
        return document;
    }

    private static DocumentContent LoadDocumentContent(DocumentFile documentFile)
    {            
        Log.Default.Trace("Load document content: {0}", documentFile.FileName);
        using var stream = new FileStream(documentFile.FileName ?? throw new InvalidOperationException("FileName is null"), FileMode.Open, FileAccess.Read);
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var documentContent = new DocumentContent() { Code = reader.ReadToEnd() };
        documentFile.ResetModified();
        return documentContent;
    }

    private void SaveCore(DocumentFile document, string fileName)
    {
        if (document.Content is null) throw new InvalidOperationException("document.Content must not be null");
        try
        {
            using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                using var writer = new StreamWriter(stream, Encoding.UTF8);
                writer.Write(document.Content.Code);
            }
            document.FileName = fileName;
            document.ResetModified();
        }
        catch (Exception ex)
        {
            Log.Default.Error("Save error: {0}", ex);
            messageService.ShowError(shellService.ShellView, Resources.SaveFileError, fileName);
        }
    }

    private async void DocumentFilePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(DocumentFile.LoadError))
        {
            await Task.Yield();
            var document = (DocumentFile)sender!;
            CloseCore(document);
            messageService.ShowError(shellService.ShellView, Resources.OpenFileError, document.FileName);
        }
    }

    private void FileServicePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IFileService.ActiveDocumentFile))
        {
            activeDocumentPropertyChangedProxy?.Remove();
            activeDocumentPropertyChangedProxy = null;

            lastActiveDocumentFile = ActiveDocumentFile;

            if (lastActiveDocumentFile != null) { activeDocumentPropertyChangedProxy = WeakEvent.PropertyChanged.Add(lastActiveDocumentFile, ActiveDocumentPropertyChanged); }
            UpdateCommands();
        }
        else if (e.PropertyName == nameof(IFileService.LockedDocumentFile))
        {
            UpdateCommands();
        }
    }

    private void ActiveDocumentPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(DocumentFile.Modified)) UpdateCommands();
    }

    private void UpdateCommands()
    {
        closeCommand.RaiseCanExecuteChanged();
        closeAllCommand.RaiseCanExecuteChanged();
        saveCommand.RaiseCanExecuteChanged();
        saveAsCommand.RaiseCanExecuteChanged();
    }

    private void ShellServiceClosing(object? sender, CancelEventArgs e)
    {
        e.Cancel = !PrepareToClose(fileService.DocumentFiles);
        if (!e.Cancel)
        {
            shellService.Settings.LastOpenedFiles = fileService.DocumentFiles.Select(x => x.FileName!).Where(x => x != null && Path.IsPathRooted(x)).ToArray();
        }
    }
}
