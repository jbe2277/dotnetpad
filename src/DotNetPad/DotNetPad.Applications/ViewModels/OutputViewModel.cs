using System.Waf.Applications;
using Waf.DotNetPad.Applications.Services;
using Waf.DotNetPad.Applications.Views;
using Waf.DotNetPad.Domain;

namespace Waf.DotNetPad.Applications.ViewModels;

public class OutputViewModel(IOutputView view, IDocumentService documentService) : ViewModel<IOutputView>(view)
{
    public IDocumentService DocumentService { get; } = documentService;

    public void AppendOutputText(DocumentFile document, string text) => ViewCore.AppendOutputText(document, text);

    public void AppendErrorText(DocumentFile document, string text) => ViewCore.AppendErrorText(document, text);

    public void ClearOutput(DocumentFile document) => ViewCore.ClearOutput(document);
}
