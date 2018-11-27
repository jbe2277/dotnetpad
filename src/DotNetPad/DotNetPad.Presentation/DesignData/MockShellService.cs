using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Waf.Foundation;
using Waf.DotNetPad.Applications.Properties;
using Waf.DotNetPad.Applications.Services;

namespace Waf.DotNetPad.Presentation.DesignData
{
    public class MockShellService : Model, IShellService
    {
        public MockShellService()
        {
            Settings = new AppSettings();
        }

        public AppSettings Settings { get; set; }
        
        public object ShellView { get; set; }

        public int Line { get; set; }
        
        public int Column { get; set; }

        public event CancelEventHandler Closing;

        protected virtual void OnClosing(CancelEventArgs e)
        {
            Closing?.Invoke(this, e);
        }
    }
}
