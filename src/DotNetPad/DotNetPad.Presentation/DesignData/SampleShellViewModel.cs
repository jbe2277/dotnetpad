using System;
using System.ComponentModel;
using Waf.DotNetPad.Applications.ViewModels;
using Waf.DotNetPad.Applications.Views;

namespace Waf.DotNetPad.Presentation.DesignData
{
    public class SampleShellViewModel : ShellViewModel
    {
        public SampleShellViewModel() : base(new MockShellView(), new MockShellService(), new MockFileService(), null!, null!)
        {
        }


        private class MockShellView : MockView, IShellView
        {
            public double VirtualScreenWidth => 0;

            public double VirtualScreenHeight => 0;

            public double Left { get; set; }

            public double Top { get; set; }

            public double Width { get; set; }

            public double Height { get; set; }

            public bool IsMaximized { get; set; }

            public double BottomPanesHeight { get; set; }


            public event CancelEventHandler? Closing;

            public event EventHandler? Closed;


            public void Show() { }

            protected virtual void OnClosing(CancelEventArgs e) => Closing?.Invoke(this, e);

            protected virtual void OnClosed(EventArgs e) => Closed?.Invoke(this, e);
        }
    }
}
