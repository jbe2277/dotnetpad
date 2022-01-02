using Waf.DotNetPad.Domain;

namespace Waf.DotNetPad.Applications.DataModels
{
    public record DocumentDataModel(DocumentFile DocumentFile, Lazy<object> LazyCodeEditorView);
}
