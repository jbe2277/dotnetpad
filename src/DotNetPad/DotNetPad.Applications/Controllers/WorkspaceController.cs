using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Host.Mef;
using System.ComponentModel.Composition;
using System.Composition.Hosting;
using System.Globalization;
using System.Waf.Applications;
using Waf.DotNetPad.Applications.CodeAnalysis;
using Waf.DotNetPad.Applications.Host;
using Waf.DotNetPad.Applications.Services;
using Waf.DotNetPad.Applications.ViewModels;
using Waf.DotNetPad.Domain;

namespace Waf.DotNetPad.Applications.Controllers;

[Export, Export(typeof(IWorkspaceService))]
internal sealed class WorkspaceController : IWorkspaceService
{
    private readonly TaskScheduler taskScheduler;
    private readonly IDocumentService documentService;
    private readonly Lazy<ShellViewModel> shellViewModel;
    private readonly Lazy<ErrorListViewModel> errorListViewModel;
    private readonly Lazy<OutputViewModel> outputViewModel;
    private readonly ThrottledAction updateDiagnosticsAction;
    private readonly DelegateTextWriter errorTextWriter;
    private readonly DelegateCommand startCommand;
    private readonly DelegateCommand stopCommand;
    private readonly DelegateCommand formatDocumentCommand;
    private readonly Dictionary<DocumentFile, DocumentId> documentIds = [];
    private Tuple<DocumentFile, BuildResult>? lastBuildResult;
    private ScriptingWorkspace workspace = null!;
    private CancellationTokenSource? updateDiagnosticsCancellation;
    private CancellationTokenSource? runScriptCancellation;
    private DocumentFile? runningDocument;

    [ImportingConstructor]
    public WorkspaceController(IDocumentService documentService, Lazy<ShellViewModel> shellViewModel, Lazy<ErrorListViewModel> errorListViewModel, Lazy<OutputViewModel> outputViewModel)
    {
        taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        this.documentService = documentService;
        this.shellViewModel = shellViewModel;
        this.errorListViewModel = errorListViewModel;
        this.outputViewModel = outputViewModel;
        updateDiagnosticsAction = new ThrottledAction(UpdateDiagnostics, ThrottledActionMode.InvokeOnlyIfIdleForDelayTime, TimeSpan.FromSeconds(2));
        Console.SetOut(new DelegateTextWriter(AppendOutputText));
        errorTextWriter = new(AppendErrorText);
        startCommand = new(StartScript, CanStartScript);
        stopCommand = new(StopScript, CanStopScript);
        formatDocumentCommand = new(FormatDocument, CanFormatDocument);
    }

    private ShellViewModel ShellViewModel => shellViewModel.Value;

    private ErrorListViewModel ErrorListViewModel => errorListViewModel.Value;

    private OutputViewModel OutputViewModel => outputViewModel.Value;

    private DocumentFile? RunningDocument
    {
        get => runningDocument;
        set
        {
            if (runningDocument == value) return;
            runningDocument = value;
            documentService.LockedDocumentFile = value;
            ShellViewModel.IsScriptRunning = runningDocument != null;
            ShellViewModel.StatusText = runningDocument is null ? null : Path.GetFileName(runningDocument.FileName) + " is running...";
            startCommand.RaiseCanExecuteChanged();
            stopCommand.RaiseCanExecuteChanged();
            formatDocumentCommand.RaiseCanExecuteChanged();
        }
    }

    public void Initialize()
    {
        using (new PerformanceTrace("new Workspace"))
        {
            workspace = new ScriptingWorkspace(CreateHostServices());
            workspace.WorkspaceChanged += WorkspaceChanged;
        }
        WeakEvent.PropertyChanged.Add(documentService, DocumentServicePropertyChanged);
        WeakEvent.CollectionChanged.Add(documentService.DocumentFiles, DocumentsCollectionChanged);
        foreach (var documentFile in documentService.DocumentFiles) AddProject(documentFile);
            
        ShellViewModel.StartCommand = startCommand;
        ShellViewModel.StopCommand = stopCommand;
        ShellViewModel.FormatDocumentCommand = formatDocumentCommand;
        ShellViewModel.ErrorListView = ErrorListViewModel.View;
        ShellViewModel.OutputView = OutputViewModel.View;
        ShellViewModel.IsErrorListViewVisible = true;
    }

    public Document GetDocument(DocumentFile documentFile)
    {
        var documentId = documentIds[documentFile];
        return workspace.CurrentSolution.GetDocument(documentId)!;
    }

    public void UpdateText(DocumentFile documentFile, string text)
    {
        if (documentFile.Content!.Code == text) return;
        var documentId = documentIds[documentFile];
        workspace.UpdateText(documentId, text);
    }

    private void WorkspaceChanged(object? sender, WorkspaceChangeEventArgs e)
    {
        if (e.Kind == WorkspaceChangeKind.DocumentChanged)
        {
            TaskHelper.Run(async () =>
            {
                var documentFile = documentIds.SingleOrDefault(x => x.Value == e.DocumentId).Key;
                if (documentFile == null) return;
                var sourceText = await GetDocument(documentFile).GetTextAsync();
                documentFile.Content!.Code = sourceText.ToString();

                if (documentService.ActiveDocumentFile == documentFile)
                {
                    ResetBuildResult(documentFile);
                    updateDiagnosticsAction.InvokeAccumulated();
                }
            }, taskScheduler);
        }
    }

    private static MefHostServices CreateHostServices()
    {
        var compositionHost = new ContainerConfiguration().WithAssemblies(MefHostServices.DefaultAssemblies).CreateContainer();
        return MefHostServices.Create(compositionHost);
    }

    private async void AddProject(DocumentFile documentFile)
    {
        await TaskUtility.WaitForProperty(documentFile, x => x.IsContentLoaded);
        using (new PerformanceTrace("AddProjectWithDocument", documentFile))
        {
            var documentId = workspace.AddProjectWithDocument(documentFile.FileName ?? "", documentFile.Content?.Code ?? "");
            documentIds.Add(documentFile, documentId);
            updateDiagnosticsAction.InvokeAccumulated();
        }
    }

    private void RemoveProject(DocumentFile documentFile)
    {
        if (documentIds.TryGetValue(documentFile, out var documentId))workspace.RemoveProject(documentId);
        documentIds.Remove(documentFile);
        ResetBuildResult(documentFile);
    }

    private void DocumentServicePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IDocumentService.ActiveDocumentFile))
        {
            updateDiagnosticsAction.Cancel();
            startCommand.RaiseCanExecuteChanged();
            formatDocumentCommand.RaiseCanExecuteChanged();
        }
    }

    private void DocumentsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            foreach (DocumentFile x in e.NewItems!) AddProject(x);
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove)
        {
            foreach (DocumentFile x in e.OldItems!) RemoveProject(x);
        }
        else if (e.Action == NotifyCollectionChangedAction.Reset && !documentService.DocumentFiles.Any())  // Clear
        {
            foreach (var x in documentIds.Keys.ToArray()) RemoveProject(x);
        }
        else throw new NotSupportedException("Collection modification is not supported!");
    }

    private async void UpdateDiagnostics()
    {
        updateDiagnosticsCancellation?.Cancel();
        updateDiagnosticsCancellation = new();
        var token = updateDiagnosticsCancellation.Token;
        try
        {
            var documentFile = documentService.ActiveDocumentFile;
            if (documentFile?.Content == null) return;
                
            token.ThrowIfCancellationRequested();
            using (new PerformanceTrace("GetDiagnosticsAsync", documentFile))
            {
                var diagnostics = await workspace.GetDiagnosticsAsync(documentIds[documentFile], token);
                token.ThrowIfCancellationRequested();
                UpdateErrorList(documentFile, diagnostics);
            }
        }
        catch (OperationCanceledException) { }
        finally
        {
            updateDiagnosticsCancellation = null;
        }
    }

    private static void UpdateErrorList(DocumentFile documentFile, IReadOnlyList<Diagnostic> diagnostics)
    {
        documentFile.Content!.ErrorList = diagnostics.Where(x => x.Severity != DiagnosticSeverity.Hidden && x.Id != "CS8632").Select(CreateErrorListItem).ToArray();
    }

    private bool CanStartScript() => RunningDocument == null && documentService.ActiveDocumentFile != null;

    private async void StartScript()
    {
        var documentFile = documentService.ActiveDocumentFile!;
        runScriptCancellation = new();
        var cancellationToken = runScriptCancellation.Token;
        RunningDocument = documentFile;

        OutputViewModel.ClearOutput(documentFile);

        if (lastBuildResult == null || lastBuildResult.Item1 != documentFile)
        {
            using (new PerformanceTrace("BuildAsync", documentFile))
            {
                var result = await workspace.BuildAsync(documentIds[documentFile], CancellationToken.None);
                UpdateErrorList(documentFile, result.Diagnostic);
                lastBuildResult = result.InMemoryAssembly == null ? null : Tuple.Create(documentFile, result);
            }
        }

        if (lastBuildResult != null)
        {
            using (new PerformanceTrace("RunScriptAsync", documentFile))
            {
                var buildResult = lastBuildResult.Item2;
                try
                {
                    await ScriptHost.RunScriptAsync(buildResult.InMemoryAssembly!, buildResult.InMemorySymbolStore!, errorTextWriter, cancellationToken);
                }
                catch (OperationCanceledException) { }
            }
            ShellViewModel.IsOutputViewVisible = true;
        }
        else
        {
            ShellViewModel.IsErrorListViewVisible = true;
        }

        RunningDocument = null;
        runScriptCancellation = null;
    }

    private bool CanStopScript() => runScriptCancellation != null && !runScriptCancellation.IsCancellationRequested;

    private void StopScript()
    {
        runScriptCancellation?.Cancel();
        ShellViewModel.StatusText = "Stopping the execution of " + Path.GetFileName(runningDocument!.FileName) + "...";
    }

    private bool CanFormatDocument() => RunningDocument == null && documentService.ActiveDocumentFile != null;

    private void FormatDocument() => workspace.FormatDocumentAsync(documentIds[documentService.ActiveDocumentFile!]);

    private void AppendOutputText(string? text) => AppendTextCore(OutputViewModel.AppendOutputText, text);

    private void AppendErrorText(string? text) => AppendTextCore(OutputViewModel.AppendErrorText, text);

    private void AppendTextCore(Action<DocumentFile, string> appendTextAction, string? text)
    {
        if (string.IsNullOrEmpty(text)) return;
        TaskHelper.Run(() =>
        {
            var document = RunningDocument;
            if (document != null) appendTextAction(document, text);
        }, taskScheduler);
    }

    private void ResetBuildResult(DocumentFile documentFile)
    {
        if (lastBuildResult?.Item1 == documentFile) lastBuildResult = null;
    }

    private static ErrorListItem CreateErrorListItem(Diagnostic diagnostic)
    {
        var mappedSpan = diagnostic.Location.GetMappedLineSpan();
        var errorSeverity = diagnostic.Severity switch
        {
            DiagnosticSeverity.Error => ErrorSeverity.Error,
            DiagnosticSeverity.Warning => ErrorSeverity.Warning,
            _ => ErrorSeverity.Info
        };
        return new ErrorListItem(errorSeverity, diagnostic.GetMessage(CultureInfo.CurrentCulture), mappedSpan.Span.Start.Line, mappedSpan.Span.Start.Character,
            mappedSpan.Span.End.Line, mappedSpan.Span.End.Character);
    }
}
