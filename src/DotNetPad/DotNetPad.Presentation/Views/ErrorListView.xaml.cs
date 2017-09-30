using System;
using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Windows;
using System.Windows.Input;
using Waf.DotNetPad.Applications.ViewModels;
using Waf.DotNetPad.Applications.Views;
using Waf.DotNetPad.Domain;

namespace Waf.DotNetPad.Presentation.Views
{
    [Export(typeof(IErrorListView))]
    public partial class ErrorListView : IErrorListView
    {
        private readonly Lazy<ErrorListViewModel> viewModel;
        

        public ErrorListView()
        {
            InitializeComponent();
            viewModel = new Lazy<ErrorListViewModel>(() => this.GetViewModel<ErrorListViewModel>());
        }


        public ErrorListViewModel ViewModel => viewModel.Value;


        private void ErrorListDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var element = e.OriginalSource as FrameworkElement;
            if (element?.DataContext is ErrorListItem)
            {
                ViewModel.GotoErrorCommand.Execute(null);
            }
        }
    }
}
