using Waf.DotNetPad.Domain;

namespace Waf.DotNetPad.Applications.Services;

public interface ICodeEditorService
{
    void SetCaret(DocumentFile documentFile, int line, int column);
}
