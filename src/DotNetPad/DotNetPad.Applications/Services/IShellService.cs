using Waf.DotNetPad.Applications.Properties;

namespace Waf.DotNetPad.Applications.Services
{
    public interface IShellService : INotifyPropertyChanged
    {
        AppSettings Settings { get; }
        
        object ShellView { get; }
        
        int Line { get; set; }

        int Column { get; set; }

        event CancelEventHandler Closing;
    }
}
