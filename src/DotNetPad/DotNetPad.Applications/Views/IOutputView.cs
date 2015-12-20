using System.Waf.Applications;
using Waf.DotNetPad.Domain;

namespace Waf.DotNetPad.Applications.Views
{
    public interface IOutputView : IView
    {
        void AppendOutputText(DocumentFile document, string text);

        void AppendErrorText(DocumentFile document, string text);

        void ClearOutput(DocumentFile document);
    }
}
