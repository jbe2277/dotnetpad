using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Waf.DotNetPad.Domain;

namespace Waf.DotNetPad.Applications.Services
{
    public interface IWorkspaceService
    {
        void UpdateText(DocumentFile documentFile, string text);
        
        Task<IReadOnlyList<ISymbol>> GetRecommendedSymbolsAsync(DocumentFile documentFile, int position, CancellationToken cancellationToken);
    }
}
