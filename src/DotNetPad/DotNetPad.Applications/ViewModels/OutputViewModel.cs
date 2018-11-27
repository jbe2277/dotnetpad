using System.ComponentModel.Composition;
using System.Waf.Applications;
using Waf.DotNetPad.Applications.Services;
using Waf.DotNetPad.Applications.Views;
using Waf.DotNetPad.Domain;

namespace Waf.DotNetPad.Applications.ViewModels
{
    [Export]
    public class OutputViewModel : ViewModel<IOutputView>
    {
        [ImportingConstructor]
        public OutputViewModel(IOutputView view, IDocumentService documentService) : base(view)
        {
            DocumentService = documentService;
        }

        public IDocumentService DocumentService { get; }

        public void AppendOutputText(DocumentFile document, string text)
        {
            ViewCore.AppendOutputText(document, text);
        }

        public void AppendErrorText(DocumentFile document, string text)
        {
            ViewCore.AppendErrorText(document, text);
        }

        public void ClearOutput(DocumentFile document)
        {
            ViewCore.ClearOutput(document);
        }
    }
}
