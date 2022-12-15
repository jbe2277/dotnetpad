using Waf.DotNetPad.Applications.ViewModels;
using Waf.DotNetPad.Applications.Views;
using Waf.DotNetPad.Domain;

namespace Waf.DotNetPad.Presentation.DesignData;

public class SampleErrorListViewModel : ErrorListViewModel
{
    public SampleErrorListViewModel() : base(new MockErrorListView(), new MockDocumentService(), null!, null!)
    {
        var documentFile = new DocumentFile(DocumentType.CSharp, "Script 1.cs", "");
        documentFile.Content!.ErrorList = new[] 
        {
            new ErrorListItem(ErrorSeverity.Info, "Info", 0, 0, 0, 0),
            new ErrorListItem(ErrorSeverity.Warning, "Warning", 3, 13, 3, 13),
            new ErrorListItem(ErrorSeverity.Error, "Error", 20, 8, 20, 8)
        };
        DocumentService.ActiveDocumentFile = documentFile;
    }


    private sealed class MockErrorListView : MockView, IErrorListView
    {
    }
}
