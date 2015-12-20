using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Waf.DotNetPad.Applications.Services
{
    public interface IShellService : INotifyPropertyChanged
    {
        AppSettings Settings { get; }
        
        object ShellView { get; }
        
        IReadOnlyCollection<Task> TasksToCompleteBeforeShutdown { get; }

        int Line { get; set; }

        int Column { get; set; }


        event CancelEventHandler Closing;


        void AddTaskToCompleteBeforeShutdown(Task task);
    }
}
