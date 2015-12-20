using System.Waf.Applications;

namespace Waf.DotNetPad.Applications.Views
{
    public interface IInfoView : IView
    {
        void ShowDialog(object owner);
    }
}
