using Waf.DotNetPad.Applications.ViewModels;
using Waf.DotNetPad.Applications.Views;
using Waf.DotNetPad.Domain;

namespace Waf.DotNetPad.Presentation.DesignData;

public class SampleCodeEditorViewModel : CodeEditorViewModel
{
    public SampleCodeEditorViewModel() : base(new MockCodeEditorView(), new MockShellService(), null!, null!)
    {
        var code = @"using System;
using System.Linq;

namespace Sample
{
    internal static class Program
    {
        internal static void Main()
        {
           Console.WriteLine(""Hello World""); 
        }
    }
}";          
        DocumentFile = new DocumentFile(DocumentType.CSharp, "Script 1.cs", code);
    }


    private class MockCodeEditorView : MockView, ICodeEditorView
    {
    }
}
