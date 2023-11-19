using Waf.DotNetPad.Domain;

namespace Waf.DotNetPad.Applications.Services;

public interface IDocumentService : INotifyPropertyChanged
{
    ReadOnlyObservableList<DocumentFile> DocumentFiles { get; }
        
    DocumentFile? ActiveDocumentFile { get; set; }

    DocumentFile? LockedDocumentFile { get; set; }
}
