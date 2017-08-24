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
using System.Waf.Presentation;
using System.Windows;
using System.Windows.Threading;
using Waf.DotNetPad.Applications.Services;
using Waf.DotNetPad.Applications.ViewModels;
using Waf.DotNetPad.Domain;
using Waf.DotNetPad.Presentation.Properties;
using Waf.DotNetPad.Presentation.Services;

namespace Waf.DotNetPad.Presentation
{
    public partial class App : Application
    {
        private AggregateCatalog catalog;
        private CompositionContainer container;
        private IEnumerable<IModuleController> moduleControllers;


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

            InitializeCultures();

            catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(WafConfiguration).Assembly));
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(ShellViewModel).Assembly));
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(App).Assembly));

            container = new CompositionContainer(catalog, CompositionOptions.DisableSilentRejection);
            CompositionBatch batch = new CompositionBatch();
            batch.AddExportedValue(container);
            container.Compose(batch);

            // Initialize all presentation services
            var presentationServices = container.GetExportedValues<IPresentationService>();
            foreach (var presentationService in presentationServices) { presentationService.Initialize(); }

            // Initialize and run all module controllers
            moduleControllers = container.GetExportedValues<IModuleController>();
            foreach (var moduleController in moduleControllers) { moduleController.Initialize(); }
            foreach (var moduleController in moduleControllers) { moduleController.Run(); }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Shutdown the module controllers in reverse order
            foreach (var moduleController in moduleControllers.Reverse()) { moduleController.Shutdown(); }

            // Wait until all registered tasks are finished
            var shellService = container.GetExportedValue<IShellService>();
            var tasksToWait = shellService.TasksToCompleteBeforeShutdown.ToArray();
            while (tasksToWait.Any(t => !t.IsCompleted && !t.IsCanceled && !t.IsFaulted))
            {
                DispatcherHelper.DoEvents();
            }

            // Dispose
            container.Dispose();
            catalog.Dispose();
            base.OnExit(e);
        }

        private static void InitializeCultures()
        {
            if (!string.IsNullOrEmpty(Settings.Default.Culture))
            {
                CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(Settings.Default.Culture);
            }
            if (!string.IsNullOrEmpty(Settings.Default.UICulture))
            {
                CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo(Settings.Default.UICulture);
            }
        }

        private static void AppDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            HandleException(e.Exception, false);
        }

        private static void AppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleException(e.ExceptionObject as Exception, e.IsTerminating);
        }

        private static void HandleException(Exception e, bool isTerminating)
        {
            if (e == null) { return; }

            Logger.Error(e.ToString());

            if (!isTerminating)
            {
                MessageBox.Show(string.Format(CultureInfo.CurrentCulture,
                        Presentation.Properties.Resources.UnknownError, e),
                    ApplicationInfo.ProductName, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
