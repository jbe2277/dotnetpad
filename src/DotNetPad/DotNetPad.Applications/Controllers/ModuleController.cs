using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Waf.Applications;
using Waf.DotNetPad.Applications.DataModels;
using Waf.DotNetPad.Applications.Services;
using Waf.DotNetPad.Applications.ViewModels;
using Waf.DotNetPad.Domain;

namespace Waf.DotNetPad.Applications.Controllers
{
    [Export(typeof(IModuleController))]
    internal class ModuleController : IModuleController
    {
        private const string appSettingsFileName = "Settings.xml";
        
        private readonly Lazy<ShellService> shellService;
        private readonly IEnvironmentService environmentService;
        private readonly ISettingsProvider settingsProvider;
        private readonly FileController fileController;
        private readonly WorkspaceController workspaceController;
        private readonly Lazy<ShellViewModel> shellViewModel;
        private readonly ExportFactory<CodeEditorViewModel> codeEditorViewModelFactory;
        private readonly ExportFactory<InfoViewModel> infoViewModelFactory;
        private readonly DelegateCommand infoCommand;
        private readonly ReadOnlyObservableCollection<DocumentDataModel> documentDataModels;
        private AppSettings appSettings;
        
        
        [ImportingConstructor]
        public ModuleController(Lazy<ShellService> shellService, IEnvironmentService environmentService, ISettingsProvider settingsProvider, 
            FileController fileController, WorkspaceController workspaceController, IFileService fileService,
            Lazy<ShellViewModel> shellViewModel, ExportFactory<CodeEditorViewModel> codeEditorViewModelFactory, ExportFactory<InfoViewModel> infoViewModelFactory)
        {
            this.shellService = shellService;
            this.environmentService = environmentService;
            this.settingsProvider = settingsProvider;
            this.fileController = fileController;
            this.workspaceController = workspaceController;
            this.shellViewModel = shellViewModel;
            this.codeEditorViewModelFactory = codeEditorViewModelFactory;
            this.infoViewModelFactory = infoViewModelFactory;
            this.infoCommand = new DelegateCommand(ShowInfo);
            this.documentDataModels = new SynchronizingCollection<DocumentDataModel, DocumentFile>(fileService.DocumentFiles, CreateDocumentDataModel);
        }


        private ShellService ShellService { get { return shellService.Value; } }

        private ShellViewModel ShellViewModel { get { return shellViewModel.Value; } }

        
        public void Initialize()
        {
            LoadSettings();

            ShellService.Settings = appSettings;

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

        public void Shutdown()
        {
            SaveSettings();
        }

        private void LoadSettings()
        {
            try
            {
                appSettings = settingsProvider.LoadSettings<AppSettings>(Path.Combine(environmentService.AppSettingsPath, appSettingsFileName));
            }
            catch (Exception ex)
            {
                Logger.Error("Could not read the settings file: {0}", ex);
                appSettings = new AppSettings();
            }
        }

        private void SaveSettings()
        {
            try
            {
                settingsProvider.SaveSettings(Path.Combine(environmentService.AppSettingsPath, appSettingsFileName), appSettings);
            }
            catch (Exception ex)
            {
                Logger.Error("Could not save the settings file: {0}", ex);
            }
        }

        private void ShowInfo()
        {
            var infoViewModel = infoViewModelFactory.CreateExport().Value;
            infoViewModel.ShowDialog(ShellService.ShellView);
        }

        private DocumentDataModel CreateDocumentDataModel(DocumentFile documentFile)
        {
            return new DocumentDataModel(documentFile, new Lazy<object>(() => CreateDocumentViewModel(documentFile).View));
        }

        private CodeEditorViewModel CreateDocumentViewModel(DocumentFile document)
        {
            var viewModel = codeEditorViewModelFactory.CreateExport().Value;
            viewModel.DocumentFile = document;
            return viewModel;
        }
    }
}
