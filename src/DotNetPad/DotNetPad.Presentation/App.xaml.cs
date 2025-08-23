using Autofac;
using System.Diagnostics;
using System.Globalization;
using System.Waf.Applications;
using System.Windows;
using System.Windows.Threading;
using Waf.DotNetPad.Applications;
using Waf.DotNetPad.Domain;
using IContainer = Autofac.IContainer;

namespace Waf.DotNetPad.Presentation;

public partial class App
{
    private IContainer? container;
    private IReadOnlyList<IModuleController> moduleControllers = [];

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

#if !DEBUG
        Log.Default.Switch.Level = SourceLevels.Information;
        DispatcherUnhandledException += AppDispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += AppDomainUnhandledException;
#else
        Log.Default.Switch.Level = SourceLevels.Verbose;
#endif
        Log.Default.Info("{0} {1} is starting; OS: {2}", ApplicationInfo.ProductName, ApplicationInfo.Version, Environment.OSVersion);
        var builder = new ContainerBuilder();
        builder.RegisterModule(new ApplicationsModule());
        builder.RegisterModule(new PresentationModule());
        container = builder.Build();

        moduleControllers = container.Resolve<IReadOnlyList<IModuleController>>();
        foreach (var x in moduleControllers) x.Initialize();
        foreach (var x in moduleControllers) x.Run();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        foreach (var x in moduleControllers.Reverse()) x.Shutdown();
        container?.Dispose();
        base.OnExit(e);
        Log.Default.Info("{0} closed", ApplicationInfo.ProductName);
    }

    private static void AppDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) => HandleException(e.Exception, false);

    private static void AppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e) => HandleException(e.ExceptionObject as Exception, e.IsTerminating);

    private static void HandleException(Exception? e, bool isTerminating)
    {
        if (e is null) return;
        Log.Default.Error(e.ToString());
        if (!isTerminating)
        {
            MessageBox.Show(string.Format(CultureInfo.CurrentCulture, Presentation.Properties.Resources.UnknownError, e), ApplicationInfo.ProductName, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
