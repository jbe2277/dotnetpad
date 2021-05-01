using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Waf;
using System.Waf.Applications;
using System.Windows;
using System.Windows.Threading;
using Waf.DotNetPad.Applications.ViewModels;
using Waf.DotNetPad.Domain;
using Waf.DotNetPad.Presentation.Services;

namespace Waf.DotNetPad.Presentation
{
    public partial class App
    {
        private AggregateCatalog catalog = null!;
        private CompositionContainer container = null!;
        private IEnumerable<IModuleController> moduleControllers = Array.Empty<IModuleController>();

        public App()
        {
            var environmentService = new EnvironmentService();
            Directory.CreateDirectory(environmentService.ProfilePath);
            ProfileOptimization.SetProfileRoot(environmentService.ProfilePath);
            ProfileOptimization.StartProfile("Startup.profile");
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

#if !(DEBUG)
            DispatcherUnhandledException += AppDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += AppDomainUnhandledException;
#endif

            catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(WafConfiguration).Assembly));
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(ShellViewModel).Assembly));
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(App).Assembly));

            container = new CompositionContainer(catalog, CompositionOptions.DisableSilentRejection);
            var batch = new CompositionBatch();
            batch.AddExportedValue(container);
            container.Compose(batch);

            moduleControllers = container.GetExportedValues<IModuleController>();
            foreach (var x in moduleControllers) x.Initialize();
            foreach (var x in moduleControllers) x.Run();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            foreach (var x in moduleControllers.Reverse()) x.Shutdown();

            container.Dispose();
            catalog.Dispose();
            base.OnExit(e);
        }

        private static void AppDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            HandleException(e.Exception, false);
        }

        private static void AppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleException(e.ExceptionObject as Exception, e.IsTerminating);
        }

        private static void HandleException(Exception? e, bool isTerminating)
        {
            if (e is null) return;
            Logger.Error(e.ToString());
            if (!isTerminating)
            {
                MessageBox.Show(string.Format(CultureInfo.CurrentCulture, Presentation.Properties.Resources.UnknownError, e),
                    ApplicationInfo.ProductName, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
