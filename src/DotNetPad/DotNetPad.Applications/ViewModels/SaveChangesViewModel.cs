using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Windows.Input;
using Waf.DotNetPad.Applications.Views;
using Waf.DotNetPad.Domain;

namespace Waf.DotNetPad.Applications.ViewModels
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class SaveChangesViewModel : ViewModel<ISaveChangesView>
    {
        private IReadOnlyList<DocumentFile> documentFiles = Array.Empty<DocumentFile>();
        private bool? dialogResult;

        [ImportingConstructor]
        public SaveChangesViewModel(ISaveChangesView view) : base(view)
        {
            YesCommand = new DelegateCommand(() => Close(true));
            NoCommand = new DelegateCommand(() => Close(false));
        }

        public static string Title => ApplicationInfo.ProductName;

        public ICommand YesCommand { get; }

        public ICommand NoCommand { get; }

        public IReadOnlyList<DocumentFile> DocumentFiles
        {
            get => documentFiles;
            set => SetProperty(ref documentFiles, value);
        }

        public bool? ShowDialog(object owner)
        {
            ViewCore.ShowDialog(owner);
            return dialogResult;
        }

        private void Close(bool? dialogResult)
        {
            this.dialogResult = dialogResult;
            ViewCore.Close();
        }
    }
}
