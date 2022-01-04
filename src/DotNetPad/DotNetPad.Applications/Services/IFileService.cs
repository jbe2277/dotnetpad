using System.Waf.Applications;
using System.Windows.Input;

namespace Waf.DotNetPad.Applications.Services;

public interface IFileService : IDocumentService
{
    ICommand NewCSharpCommand { get; }

    ICommand NewVisualBasicCommand { get; }

    DelegateCommand NewCSharpFromClipboardCommand { get; }

    DelegateCommand NewVisualBasicFromClipboardCommand { get; }

    ICommand OpenCommand { get; }

    ICommand CloseCommand { get; }

    ICommand CloseAllCommand { get; }

    ICommand SaveCommand { get; }

    ICommand SaveAsCommand { get; }
}
