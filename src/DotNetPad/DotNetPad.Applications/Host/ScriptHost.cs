using System.Reflection;
using System.Runtime.Loader;

namespace Waf.DotNetPad.Applications.Host
{
    // Use AssemblyLoadContext for unloading the script assembly but this does not provide any kind of isolation.
    public static class ScriptHost
    {
        public static Task RunScriptAsync(byte[] inMemoryAssembly, byte[] inMemorySymbolStore, TextWriter errorTextWriter, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                try
                {
                    var context = new ScriptLoadContext();
                    var scriptAssembly = context.LoadFromStream(new MemoryStream(inMemoryAssembly), new MemoryStream(inMemorySymbolStore));
                    if (scriptAssembly.EntryPoint is null) throw new InvalidOperationException("Could not find the entry point of the script assembly.");
                    scriptAssembly.EntryPoint.Invoke(null, null);
                    context.Unload();
                }
                catch (Exception ex)
                {
                    errorTextWriter.WriteLine(ex.ToString());
                }
            }, cancellationToken);
        }

        private class ScriptLoadContext : AssemblyLoadContext
        {
            public ScriptLoadContext() : base(isCollectible: true) { }

            // Return null so that dependant assemblies are loaded into the default context.
            protected override Assembly? Load(AssemblyName assemblyName) => null;
        }
    }
}
