using System.Collections.Generic;
using System.Waf.Applications;
using System.Waf.Foundation;
using System.Windows.Input;
using Waf.DotNetPad.Applications.Services;
using Waf.DotNetPad.Domain;

namespace Waf.DotNetPad.Presentation.DesignData
{
    public class MockFileService : Model, IFileService
    {
        public ICommand NewCSharpCommand { get; set; }
        
        public ICommand NewVisualBasicCommand { get; set; }
        
        public DelegateCommand NewCSharpFromClipboardCommand { get; set; }
        
        public DelegateCommand NewVisualBasicFromClipboardCommand { get; set; }
        
        public ICommand OpenCommand { get; set; }
        
        public ICommand CloseCommand { get; set; }
        
        public ICommand CloseAllCommand { get; set; }
        
        public ICommand SaveCommand { get; set; }
        
        public ICommand SaveAsCommand { get; set; }

        public IReadOnlyObservableList<DocumentFile> DocumentFiles { get; set; }
        
        public DocumentFile ActiveDocumentFile { get; set; }
        
        public DocumentFile LockedDocumentFile { get; set; }
    }
}
