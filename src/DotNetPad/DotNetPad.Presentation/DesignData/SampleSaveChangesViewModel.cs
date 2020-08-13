using Waf.DotNetPad.Applications.ViewModels;
using Waf.DotNetPad.Applications.Views;
using Waf.DotNetPad.Domain;

namespace Waf.DotNetPad.Presentation.DesignData
{
    public class SampleSaveChangesViewModel : SaveChangesViewModel
    {
        public SampleSaveChangesViewModel() : base(new MockSaveChangesView())
        {
            DocumentFiles = new[]
            {
                new DocumentFile(DocumentType.CSharp, null!) { FileName = "Script 1.cs" },
                new DocumentFile(DocumentType.CSharp, null!) { FileName = "Script 2.cs" },
                new DocumentFile(DocumentType.VisualBasic, null!) { FileName = @"C:\Users\Luke\Documents\Waf DotNetPad\Script 3.vb" },
            };
        }


        private class MockSaveChangesView : MockView, ISaveChangesView
        {
            public void ShowDialog(object owner)
            {
            }

            public void Close()
            {
            }
        }
    }
}
