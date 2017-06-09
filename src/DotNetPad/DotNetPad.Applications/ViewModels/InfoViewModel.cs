using Microsoft.Win32;
using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Waf.Applications;
using System.Windows.Input;
using Waf.DotNetPad.Applications.Views;
using Waf.DotNetPad.Domain;

namespace Waf.DotNetPad.Applications.ViewModels
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class InfoViewModel : ViewModel<IInfoView>
    {
        private readonly DelegateCommand showWebsiteCommand;


        [ImportingConstructor]
        public InfoViewModel(IInfoView view)
            : base(view)
        {
            showWebsiteCommand = new DelegateCommand(ShowWebsite);
        }


        public ICommand ShowWebsiteCommand => showWebsiteCommand;

        public string ProductName => ApplicationInfo.ProductName;

        public string Version => ApplicationInfo.Version;

        public string OSVersion => Environment.OSVersion.ToString();

        public string NetVersion { get; } = GetDotNetVersion();

        public bool Is64BitProcess => Environment.Is64BitProcess;


        public void ShowDialog(object owner)
        {
            ViewCore.ShowDialog(owner);
        }

        private void ShowWebsite(object parameter)
        {
            string url = (string)parameter;
            try
            {
                Process.Start(url);
            }
            catch (Exception e)
            {
                Logger.Error("An exception occured when trying to show the url '{0}'. Exception: {1}", url, e);
            }
        }

        private static string GetDotNetVersion()
        {
            using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
            using (var key = baseKey.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\"))
            {
                int? releaseKey = (int?)key?.GetValue("Release");
                string majorVersion = "";

                if (releaseKey > 460805) majorVersion = "4.7 or later";
                else if (releaseKey >= 460798) majorVersion = "4.7";
                else if (releaseKey >= 394802) majorVersion = "4.6.2";
                else if (releaseKey >= 394254) majorVersion = "4.6.1";

                if (releaseKey != null) majorVersion += " (" + releaseKey + ")";
                return majorVersion;
            }
        }
    }
}
