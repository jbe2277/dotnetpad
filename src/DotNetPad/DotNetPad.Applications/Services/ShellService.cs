using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Waf.Foundation;
using Waf.DotNetPad.Applications.Views;

namespace Waf.DotNetPad.Applications.Services
{
    [Export(typeof(IShellService)), Export]
    internal class ShellService : Model, IShellService
    {
        private readonly Lazy<IShellView> shellView;
        private readonly List<Task> tasksToCompleteBeforeShutdown;
        private int line;
        private int column;
        private bool isClosingEventInitialized;
        private event CancelEventHandler closing;


        [ImportingConstructor]
        public ShellService(Lazy<IShellView> shellView)
        {
            this.shellView = shellView;
            this.tasksToCompleteBeforeShutdown = new List<Task>();
        }


        public AppSettings Settings { get; set; }
        
        public object ShellView { get { return shellView.Value; } }

        public IReadOnlyCollection<Task> TasksToCompleteBeforeShutdown { get { return tasksToCompleteBeforeShutdown; } }

        public int Line
        {
            get { return line; }
            set { SetProperty(ref line, value); }
        }

        public int Column
        {
            get { return column; }
            set { SetProperty(ref column, value); }
        }


        public event CancelEventHandler Closing
        {
            add
            {
                closing += value;
                InitializeClosingEvent();
            }
            remove { closing -= value; }
        }


        public void AddTaskToCompleteBeforeShutdown(Task task)
        {
            tasksToCompleteBeforeShutdown.Add(task);
        }

        protected virtual void OnClosing(CancelEventArgs e)
        {
            if (closing != null) { closing(this, e); }
        }

        private void InitializeClosingEvent()
        {
            if (isClosingEventInitialized) { return; }

            isClosingEventInitialized = true;
            shellView.Value.Closing += ShellViewClosing;
        }

        private void ShellViewClosing(object sender, CancelEventArgs e)
        {
            OnClosing(e);
        }
    }
}
