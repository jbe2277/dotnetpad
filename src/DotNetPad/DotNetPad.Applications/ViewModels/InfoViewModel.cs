using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Waf.Applications;
using System.Windows.Input;
using Waf.DotNetPad.Applications.Views;
using Waf.DotNetPad.Domain;

namespace Waf.DotNetPad.Applications.ViewModels
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class InfoViewModel : ViewModel<IInfoView>
    {
        [ImportingConstructor]
        public InfoViewModel(IInfoView view) : base(view)
        {
            ShowWebsiteCommand = new DelegateCommand(ShowWebsite);
        }

        public ICommand ShowWebsiteCommand { get; }

        public string ProductName => ApplicationInfo.ProductName;

        public string Version => ApplicationInfo.Version;

        public string OSVersion => Environment.OSVersion.ToString();

        public string NetVersion { get; } = Environment.Version.ToString();

        public Architecture ProcessArchitecture => RuntimeInformation.ProcessArchitecture;

        public void ShowDialog(object owner) => ViewCore.ShowDialog(owner);
        
        private void ShowWebsite(object? parameter)
        {
            var url = (parameter as string) ?? "";
            try
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch (Exception e)
            {
                Logger.Error("An exception occured when trying to show the url '{0}'. Exception: {1}", url, e);
            }
        }
    }
}
