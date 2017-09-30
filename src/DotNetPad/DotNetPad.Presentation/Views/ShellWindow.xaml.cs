using System;
using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Windows;
using Waf.DotNetPad.Applications.ViewModels;
using Waf.DotNetPad.Applications.Views;

namespace Waf.DotNetPad.Presentation.Views
{
    [Export(typeof(IShellView))]
    public partial class ShellWindow : IShellView
    {
        private readonly Lazy<ShellViewModel> viewModel;
        
        
        public ShellWindow()
        {
            InitializeComponent();
            viewModel = new Lazy<ShellViewModel>(() => this.GetViewModel<ShellViewModel>());
        }


        public double VirtualScreenWidth => SystemParameters.VirtualScreenWidth;

        public double VirtualScreenHeight => SystemParameters.VirtualScreenHeight;

        public bool IsMaximized
        {
            get { return WindowState == WindowState.Maximized; }
            set
            {
                if (value)
                {
                    WindowState = WindowState.Maximized;
                }
                else if (WindowState == WindowState.Maximized)
                {
                    WindowState = WindowState.Normal;
                }
            }
        }

        public double BottomPanesHeight
        {
            get { return bottomPanesRow.Height.Value; }
            set { bottomPanesRow.Height = new GridLength(value); }
        }

        private ShellViewModel ViewModel => viewModel.Value;


        private void NewFileButtonClick(object sender, RoutedEventArgs e)
        {
            newFilePopup.IsOpen = true;
        }

        private void NewFilePopupOpened(object sender, EventArgs e)
        {
            ViewModel.FileService.NewCSharpFromClipboardCommand.RaiseCanExecuteChanged();
            ViewModel.FileService.NewVisualBasicFromClipboardCommand.RaiseCanExecuteChanged();
        }

        private void CloseFilePopup(object sender, RoutedEventArgs e)
        {
            newFilePopup.IsOpen = false;
        }

        private void MoreButtonClick(object sender, RoutedEventArgs e)
        {
            morePopup.IsOpen = true;
        }

        private void CloseMorePopup(object sender, RoutedEventArgs e)
        {
            morePopup.IsOpen = false;
        }

        private void SamplesButtonClick(object sender, RoutedEventArgs e)
        {
            samplesPopup.IsOpen = true;
        }

        private void CloseSamplesPopup(object sender, RoutedEventArgs e)
        {
            samplesPopup.IsOpen = false;
        }
    }
}
