using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Waf.DotNetPad.Applications.Host
{
    [Export]
    public class ScriptHost
    {
        private readonly object fieldsLock = new object();
        private byte[] loadedAssembly;
        private AppDomain scriptAppDomain;
        private RemoteScriptRun remoteScriptRun;

        public async Task RunScriptAsync(byte[] inMemoryAssembly, byte[] inMemorySymbolStore, TextWriter outputTextWriter, TextWriter errorTextWriter, CancellationToken cancellationToken)
        {
            cancellationToken.Register(StopScriptCore);
            await Task.Run(() => RunScriptCore(inMemoryAssembly, inMemorySymbolStore, outputTextWriter, errorTextWriter), cancellationToken);
        }

        private void RunScriptCore(byte[] inMemoryAssembly, byte[] inMemorySymbolStore, TextWriter outputTextWriter, TextWriter errorTextWriter)
        {
            RemoteScriptRun scriptRun;
            lock (fieldsLock)
            {
                if (inMemoryAssembly != loadedAssembly)
                {
                    if (scriptAppDomain != null)
                    {
                        AppDomain.Unload(scriptAppDomain);
                    }
                    scriptAppDomain = AppDomain.CreateDomain("ScriptAppDomain");
                    remoteScriptRun = (RemoteScriptRun)scriptAppDomain.CreateInstanceAndUnwrap(typeof(RemoteScriptRun).Assembly.FullName, typeof(RemoteScriptRun).FullName);
                    remoteScriptRun.Load(inMemoryAssembly, inMemorySymbolStore, outputTextWriter, errorTextWriter);
                    loadedAssembly = inMemoryAssembly;
                }
                scriptRun = remoteScriptRun;
            }
            
            try
            {
                scriptRun?.Run();
            }
            catch (AppDomainUnloadedException)
            {
            }
        }

        private void StopScriptCore()
        {
            lock (fieldsLock)
            {
                if (scriptAppDomain != null)
                {
                    AppDomain.Unload(scriptAppDomain);
                    scriptAppDomain = null;
                    loadedAssembly = null;
                    remoteScriptRun = null;
                }
            }
        }
    }
}
