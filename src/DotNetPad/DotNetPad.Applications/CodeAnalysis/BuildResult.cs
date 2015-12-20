using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace Waf.DotNetPad.Applications.CodeAnalysis
{
    public class BuildResult
    {
        private readonly IReadOnlyList<Diagnostic> diagnostic;
        private readonly byte[] inMemoryAssembly;
        private readonly byte[] inMemorySymbolStore;


        public BuildResult(IReadOnlyList<Diagnostic> diagnostic, byte[] inMemoryAssembly, byte[] inMemorySymbolStore)
        {
            this.diagnostic = diagnostic;
            this.inMemoryAssembly = inMemoryAssembly;
            this.inMemorySymbolStore = inMemorySymbolStore;
        }


        public IReadOnlyList<Diagnostic> Diagnostic { get { return diagnostic; } }

        public byte[] InMemoryAssembly { get { return inMemoryAssembly; } }

        public byte[] InMemorySymbolStore { get { return inMemorySymbolStore; } }
    }
}
