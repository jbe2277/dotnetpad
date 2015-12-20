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
        private DocumentFile documentFile;
        

        [ImportingConstructor]
        public CodeEditorViewModel(ICodeEditorView view, IShellService shellService, IWorkspaceService workspaceService, CodeEditorService codeEditorService)
            : base(view)
        {
            ShellService = shellService;
            WorkspaceService = workspaceService;
            CodeEditorService = codeEditorService;
        }


        public IShellService ShellService { get; }

        public IWorkspaceService WorkspaceService { get; }

        public CodeEditorService CodeEditorService { get; }

        public DocumentFile DocumentFile
        {
            get { return documentFile; }
            set { SetProperty(ref documentFile, value); }
        }
    }
}
