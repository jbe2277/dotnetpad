using Waf.DotNetPad.Applications.ViewModels;
using Waf.DotNetPad.Applications.Views;
using Waf.DotNetPad.Domain;

namespace Waf.DotNetPad.Presentation.DesignData;

public class SampleSaveChangesViewModel : SaveChangesViewModel
{
    public SampleSaveChangesViewModel() : base(new MockSaveChangesView())
    {
        DocumentFiles = [
            new(DocumentType.CSharp, null!) { FileName = "Script 1.cs" },
            new(DocumentType.CSharp, null!) { FileName = "Script 2.cs" },
            new(DocumentType.VisualBasic, null!) { FileName = @"C:\Users\Luke\Documents\Waf DotNetPad\Script 3.vb" },
        ];
    }


    private sealed class MockSaveChangesView : MockView, ISaveChangesView
    {
        public void ShowDialog(object owner) { }

        public void Close() { }
    }
}
