using System.ComponentModel.Composition;
using System.Windows;
using Waf.DotNetPad.Applications.Views;

namespace Waf.DotNetPad.Presentation.Views
{
    [Export(typeof(ISaveChangesView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class SaveChangesWindow : ISaveChangesView
    {
        public SaveChangesWindow()
        {
            InitializeComponent();
        }


        public void ShowDialog(object owner)
        {
            Owner = owner as Window;
            ShowDialog();
        }
    }
}
