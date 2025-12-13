using System.Waf.Applications;
using System.Windows.Input;
using Waf.DotNetPad.Applications.DataModels;
using Waf.DotNetPad.Applications.Properties;
using Waf.DotNetPad.Applications.Services;
using Waf.DotNetPad.Applications.Views;

namespace Waf.DotNetPad.Applications.ViewModels;

public class ShellViewModel : ViewModel<IShellView>
{
    private readonly AppSettings settings;
    private readonly DelegateCommand garbageCollectorCommand;

    public ShellViewModel(IShellView view, IShellService shellService, IFileService fileService, CSharpSampleService csharpSampleService, VisualBasicSampleService visualBasicSampleService) : base(view)
    {
        ShellService = shellService;
        FileService = fileService;
        CSharpSampleService = csharpSampleService;
        VisualBasicSampleService = visualBasicSampleService;
        settings = shellService.Settings;
        garbageCollectorCommand = new(GC.Collect);

        WeakEvent.PropertyChanged.Add(fileService, FileServicePropertyChanged);
        view.Closed += ViewClosed;

        // Restore the window size when the values are valid.
        if (settings.Left >= 0 && settings.Top >= 0 && settings.Width > 0 && settings.Height > 0
            && settings.Left + settings.Width <= view.VirtualScreenWidth
            && settings.Top + settings.Height <= view.VirtualScreenHeight)
        {
            view.Left = settings.Left;
            view.Top = settings.Top;
            view.Height = settings.Height;
            view.Width = settings.Width;
            view.BottomPanesHeight = settings.BottomPanesHeight;
        }
        view.IsMaximized = settings.IsMaximized;
    }

    public string Title => ApplicationInfo.ProductName;

    public IShellService ShellService { get; }

    public IFileService FileService { get; }

    public CSharpSampleService CSharpSampleService { get; }

    public VisualBasicSampleService VisualBasicSampleService { get; }

    public object? ErrorListView { get; set => SetProperty(ref field, value); }

    public object? OutputView { get; set => SetProperty(ref field, value); }

    public ICommand? StartCommand { get; set => SetProperty(ref field, value); }

    public ICommand? StopCommand { get; set => SetProperty(ref field, value); }

    public ICommand? FormatDocumentCommand { get; set => SetProperty(ref field, value); }

    public bool IsScriptRunning { get; set => SetProperty(ref field, value); }

    public ICommand InfoCommand { get; set => SetProperty(ref field, value); } = DelegateCommand.DisabledCommand;

    public ICommand GarbageCollectorCommand => garbageCollectorCommand;

    public object? CurrentStatusView { get; private set => SetProperty(ref field, value); }

    public bool IsErrorListViewVisible
    {
        get => CurrentStatusView == ErrorListView;
        set { if (value) { CurrentStatusView = ErrorListView; } }
    }

    public bool IsOutputViewVisible
    {
        get => CurrentStatusView == OutputView;
        set { if (value) { CurrentStatusView = OutputView; } }
    }

    public IReadOnlyList<DocumentDataModel> DocumentDataModels { get; set => SetProperty(ref field, value); } = [];

    public DocumentDataModel? ActiveDocumentDataModel
    {
        get;
        set
        {
            if (SetProperty(ref field, value) && value != null)
            {
                FileService.ActiveDocumentFile = value.DocumentFile;
            }
        }
    }

    public string? StatusText { get; set => SetProperty(ref field, value ?? Resources.Ready); } = Resources.Ready;

    public void Show() => ViewCore.Show();

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(CurrentStatusView))
        {
            RaisePropertyChanged(nameof(IsErrorListViewVisible), nameof(IsOutputViewVisible));
        }
    }

    private void FileServicePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IFileService.ActiveDocumentFile))
        {
            ActiveDocumentDataModel = DocumentDataModels.FirstOrDefault(x => x.DocumentFile == FileService.ActiveDocumentFile);
        }
    }

    private void ViewClosed(object? sender, EventArgs e)
    {
        settings.Left = ViewCore.Left;
        settings.Top = ViewCore.Top;
        settings.Height = ViewCore.Height;
        settings.Width = ViewCore.Width;
        settings.IsMaximized = ViewCore.IsMaximized;
        settings.BottomPanesHeight = ViewCore.BottomPanesHeight;
    }
}
