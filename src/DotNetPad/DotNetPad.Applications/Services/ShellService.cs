using Waf.DotNetPad.Applications.Properties;
using Waf.DotNetPad.Applications.Views;

namespace Waf.DotNetPad.Applications.Services;

internal sealed class ShellService(Lazy<IShellView> shellView) : Model, IShellService
{
    private bool isClosingEventInitialized;
    private CancelEventHandler? closing;

    public AppSettings Settings { get; set; } = null!;
        
    public object ShellView => shellView.Value;

    public int Line { get; set => SetProperty(ref field, value); }

    public int Column { get; set => SetProperty(ref field, value); }

    public event CancelEventHandler? Closing
    {
        add
        {
            closing += value;
            InitializeClosingEvent();
        }
        remove { closing -= value; }
    }

    private void OnClosing(CancelEventArgs e) => closing?.Invoke(this, e);

    private void InitializeClosingEvent()
    {
        if (isClosingEventInitialized) return;
        isClosingEventInitialized = true;
        shellView.Value.Closing += ShellViewClosing;
    }

    private void ShellViewClosing(object? sender, CancelEventArgs e) => OnClosing(e);
}
