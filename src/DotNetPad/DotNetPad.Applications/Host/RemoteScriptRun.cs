using System;
using System.IO;
using System.Reflection;

namespace Waf.DotNetPad.Applications.Host
{
    public class RemoteScriptRun : MarshalByRefObject
    {
        private Assembly? scriptAssembly;

        public void Load(byte[] inMemoryAssembly, byte[] inMemorySymbolStore, TextWriter outputTextWriter, TextWriter errorTextWriter)
        {
            if (scriptAssembly != null) { throw new InvalidOperationException("Assembly is already loaded."); }

            scriptAssembly = Assembly.Load(inMemoryAssembly, inMemorySymbolStore);
            if (outputTextWriter != null)
            {
                Console.SetOut(outputTextWriter);
            }
            if (errorTextWriter != null)
            {
                Console.SetError(errorTextWriter);
            }
        }

        public void Run()
        {
            if (scriptAssembly == null) { throw new InvalidOperationException("The script assembly must first be loaded."); }
            try
            {
                scriptAssembly.EntryPoint.Invoke(null, null);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }
        }

        public override object? InitializeLifetimeService()
        {
            return null;
        }
    }
}
