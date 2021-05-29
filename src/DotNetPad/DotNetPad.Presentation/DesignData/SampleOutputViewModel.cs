using Waf.DotNetPad.Applications.ViewModels;
using Waf.DotNetPad.Applications.Views;
using Waf.DotNetPad.Domain;

namespace Waf.DotNetPad.Presentation.DesignData
{
    public class SampleOutputViewModel : OutputViewModel
    {
        public SampleOutputViewModel() : base(new MockOutputView(), new MockFileService())
        {
        }


        private class MockOutputView : MockView, IOutputView
        {
            public void AppendOutputText(DocumentFile document, string text)
            {
            }

            public void AppendErrorText(DocumentFile document, string text)
            {
            }

            public void ClearOutput(DocumentFile document)
            {
            }
        }
    }
}
