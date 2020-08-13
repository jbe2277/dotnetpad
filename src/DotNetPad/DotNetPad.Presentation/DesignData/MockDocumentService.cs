using System.Waf.Foundation;
using Waf.DotNetPad.Applications.Services;
using Waf.DotNetPad.Domain;

namespace Waf.DotNetPad.Presentation.DesignData
{
    internal class MockDocumentService : Model, IDocumentService
    {
        public IReadOnlyObservableList<DocumentFile> DocumentFiles { get; set; } = null!;
        
        public DocumentFile? ActiveDocumentFile { get; set; }

        public DocumentFile? LockedDocumentFile { get; set; }
    }
}
