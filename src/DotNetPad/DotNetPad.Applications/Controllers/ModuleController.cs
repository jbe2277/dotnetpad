using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Waf.Applications.Services;
using Waf.DotNetPad.Applications.DataModels;
using Waf.DotNetPad.Applications.Properties;
using Waf.DotNetPad.Applications.Services;
using Waf.DotNetPad.Applications.ViewModels;
using Waf.DotNetPad.Domain;

namespace Waf.DotNetPad.Applications.Controllers;

[Export(typeof(IModuleController))]
internal class ModuleController : IModuleController
{
    private readonly Lazy<ShellService> shellService;
    private readonly ISettingsService settingsService;
    private readonly FileController fileController;
    private readonly WorkspaceController workspaceController;
    private readonly Lazy<ShellViewModel> shellViewModel;
    private readonly ExportFactory<CodeEditorViewModel> codeEditorViewModelFactory;
    private readonly ExportFactory<InfoViewModel> infoViewModelFactory;
    private readonly DelegateCommand infoCommand;
    private readonly ReadOnlyObservableCollection<DocumentDataModel> documentDataModels;
        
    [ImportingConstructor]
    public ModuleController(Lazy<ShellService> shellService, ISettingsService settingsService, FileController fileController, WorkspaceController workspaceController, IFileService fileService,
        Lazy<ShellViewModel> shellViewModel, ExportFactory<CodeEditorViewModel> codeEditorViewModelFactory, ExportFactory<InfoViewModel> infoViewModelFactory)
    {
        this.shellService = shellService;
        this.settingsService = settingsService;
        this.fileController = fileController;
        this.workspaceController = workspaceController;
        this.shellViewModel = shellViewModel;
        this.codeEditorViewModelFactory = codeEditorViewModelFactory;
        this.infoViewModelFactory = infoViewModelFactory;
        infoCommand = new DelegateCommand(ShowInfo);
        documentDataModels = new SynchronizingCollection<DocumentDataModel, DocumentFile>(fileService.DocumentFiles, CreateDocumentDataModel);
    }

    private ShellService ShellService => shellService.Value;

    private ShellViewModel ShellViewModel => shellViewModel.Value;

    public void Initialize()
    {
        settingsService.ErrorOccurred += (sender, e) => Log.Default.Error("Error in SettingsService: {0}", e.Error);
        ShellService.Settings = settingsService.Get<AppSettings>();

        fileController.Initialize();
        workspaceController.Initialize();

        ShellViewModel.InfoCommand = infoCommand;
        ShellViewModel.DocumentDataModels = documentDataModels;
    }

    public void Run()
    {
        fileController.Run();
        ShellViewModel.Show();
    }

    public void Shutdown() { }

    private void ShowInfo()
    {
        var infoViewModel = infoViewModelFactory.CreateExport().Value;
        infoViewModel.ShowDialog(ShellService.ShellView);
    }

    private DocumentDataModel CreateDocumentDataModel(DocumentFile documentFile) => new(documentFile, new Lazy<object>(() => CreateDocumentViewModel(documentFile).View));

    private CodeEditorViewModel CreateDocumentViewModel(DocumentFile document)
    {
        var viewModel = codeEditorViewModelFactory.CreateExport().Value;
        viewModel.DocumentFile = document;
        return viewModel;
    }
}
