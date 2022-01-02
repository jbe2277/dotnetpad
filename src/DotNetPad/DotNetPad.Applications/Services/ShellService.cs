using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Waf.Foundation;
using Waf.DotNetPad.Applications.Properties;
using Waf.DotNetPad.Applications.Views;

namespace Waf.DotNetPad.Applications.Services
{
    [Export(typeof(IShellService)), Export]
    internal class ShellService : Model, IShellService
    {
        private readonly Lazy<IShellView> shellView;
        private int line;
        private int column;
        private bool isClosingEventInitialized;
        private event CancelEventHandler? closing;

        [ImportingConstructor]
        public ShellService(Lazy<IShellView> shellView)
        {
            this.shellView = shellView;
        }

        public AppSettings Settings { get; set; } = null!;
        
        public object ShellView => shellView.Value;

        public int Line
        {
            get => line;
            set => SetProperty(ref line, value);
        }

        public int Column
        {
            get => column;
            set => SetProperty(ref column, value);
        }

        public event CancelEventHandler? Closing
        {
            add
            {
                closing += value;
                InitializeClosingEvent();
            }
            remove { closing -= value; }
        }

        protected virtual void OnClosing(CancelEventArgs e)
        {
            closing?.Invoke(this, e);
        }

        private void InitializeClosingEvent()
        {
            if (isClosingEventInitialized) return;
            isClosingEventInitialized = true;
            shellView.Value.Closing += ShellViewClosing;
        }

        private void ShellViewClosing(object? sender, CancelEventArgs e) => OnClosing(e);
    }
}
