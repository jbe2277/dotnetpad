using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace Waf.DotNetPad.Applications.CodeAnalysis
{
    public record BuildResult(IReadOnlyList<Diagnostic> Diagnostic, byte[]? InMemoryAssembly, byte[]? InMemorySymbolStore);
}
