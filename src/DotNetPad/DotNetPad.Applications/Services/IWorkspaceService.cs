using Microsoft.CodeAnalysis;
using Waf.DotNetPad.Domain;

namespace Waf.DotNetPad.Applications.Services
{
    public interface IWorkspaceService
    {
        Workspace Workspace { get; }

        Document GetDocument(DocumentFile documentFile);

        void UpdateText(DocumentFile documentFile, string text);
    }
}
