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

        public string NetVersion => Environment.Version.ToString();

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
    }
}
