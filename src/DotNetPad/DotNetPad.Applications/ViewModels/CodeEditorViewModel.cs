using System.ComponentModel.Composition;
using System.Waf.Applications;
using Waf.DotNetPad.Applications.Services;
using Waf.DotNetPad.Applications.Views;
using Waf.DotNetPad.Domain;

namespace Waf.DotNetPad.Applications.ViewModels
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class CodeEditorViewModel : ViewModel<ICodeEditorView>
    {
        private readonly IShellService shellService;
        private readonly IWorkspaceService workspaceService;
        private readonly CodeEditorService codeEditorService;
        private DocumentFile documentFile;
        

        [ImportingConstructor]
        public CodeEditorViewModel(ICodeEditorView view, IShellService shellService, IWorkspaceService workspaceService, CodeEditorService codeEditorService)
            : base(view)
        {
            this.shellService = shellService;
            this.workspaceService = workspaceService;
            this.codeEditorService = codeEditorService;
        }


        public IShellService ShellService { get { return shellService; } }

        public IWorkspaceService WorkspaceService { get { return workspaceService; } }

        public CodeEditorService CodeEditorService { get { return codeEditorService; } }

        public DocumentFile DocumentFile
        {
            get { return documentFile; }
            set { SetProperty(ref documentFile, value); }
        }
    }
}
