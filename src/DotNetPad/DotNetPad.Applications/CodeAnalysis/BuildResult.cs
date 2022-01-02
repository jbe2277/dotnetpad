using Microsoft.CodeAnalysis;

namespace Waf.DotNetPad.Applications.CodeAnalysis
{
    public record BuildResult(IReadOnlyList<Diagnostic> Diagnostic, byte[]? InMemoryAssembly, byte[]? InMemorySymbolStore);
}
