using System.Waf.Applications;
using System.Waf.Foundation;
using System.Windows.Input;
using Waf.DotNetPad.Applications.Services;
using Waf.DotNetPad.Domain;

namespace Waf.DotNetPad.Presentation.DesignData
{
    public class MockFileService : Model, IFileService
    {
        public ICommand NewCSharpCommand { get; set; } = null!;

        public ICommand NewVisualBasicCommand { get; set; } = null!;

        public DelegateCommand NewCSharpFromClipboardCommand { get; set; } = null!;

        public DelegateCommand NewVisualBasicFromClipboardCommand { get; set; } = null!;

        public ICommand OpenCommand { get; set; } = null!;

        public ICommand CloseCommand { get; set; } = null!;

        public ICommand CloseAllCommand { get; set; } = null!;

        public ICommand SaveCommand { get; set; } = null!;

        public ICommand SaveAsCommand { get; set; } = null!;

        public IReadOnlyObservableList<DocumentFile> DocumentFiles { get; set; } = null!;
        
        public DocumentFile? ActiveDocumentFile { get; set; }
        
        public DocumentFile? LockedDocumentFile { get; set; }
    }
}
