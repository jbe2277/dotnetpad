using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace Waf.DotNetPad.Applications.CodeAnalysis
{
    public class BuildResult
    {
        public BuildResult(IReadOnlyList<Diagnostic> diagnostic, byte[]? inMemoryAssembly, byte[]? inMemorySymbolStore)
        {
            Diagnostic = diagnostic;
            InMemoryAssembly = inMemoryAssembly;
            InMemorySymbolStore = inMemorySymbolStore;
        }

        public IReadOnlyList<Diagnostic> Diagnostic { get; }

        public byte[]? InMemoryAssembly { get; }

        public byte[]? InMemorySymbolStore { get; }
    }
}
