using System.Windows;
using Waf.DotNetPad.Applications.Views;

namespace Waf.DotNetPad.Presentation.Views;

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
