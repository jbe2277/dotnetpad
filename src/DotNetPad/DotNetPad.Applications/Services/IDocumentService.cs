using System.ComponentModel;
using System.Waf.Foundation;
using Waf.DotNetPad.Domain;

namespace Waf.DotNetPad.Applications.Services
{
    public interface IDocumentService : INotifyPropertyChanged
    {
        IReadOnlyObservableList<DocumentFile> DocumentFiles { get; }
        
        DocumentFile? ActiveDocumentFile { get; set; }

        DocumentFile? LockedDocumentFile { get; set; }
    }
}
