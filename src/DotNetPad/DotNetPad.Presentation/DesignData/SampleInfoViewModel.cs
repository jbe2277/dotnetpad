using Waf.DotNetPad.Applications.ViewModels;
using Waf.DotNetPad.Applications.Views;

namespace Waf.DotNetPad.Presentation.DesignData
{
    public class SampleInfoViewModel : InfoViewModel
    {
        public SampleInfoViewModel() : base(new MockInfoView())
        {
        }


        private class MockInfoView : MockView, IInfoView
        {
            public void ShowDialog(object owner)
            {
            }
        }
    }
}
