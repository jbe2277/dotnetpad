using System.Waf.Applications;
using Waf.DotNetPad.Applications.Services;
using Waf.DotNetPad.Applications.Views;
using Waf.DotNetPad.Domain;

namespace Waf.DotNetPad.Applications.ViewModels;

public class CodeEditorViewModel(ICodeEditorView view, IShellService shellService, IWorkspaceService workspaceService, CodeEditorService codeEditorService) 
    : ViewModel<ICodeEditorView>(view)
{
    public IShellService ShellService { get; } = shellService;

    public IWorkspaceService WorkspaceService { get; } = workspaceService;

    public CodeEditorService CodeEditorService { get; } = codeEditorService;

    public DocumentFile DocumentFile { get; set => SetProperty(ref field, value); } = null!;
}
