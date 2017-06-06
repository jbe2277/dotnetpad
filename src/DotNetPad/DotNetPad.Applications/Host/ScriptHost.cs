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
        private string oldCurrentDirectory;


        public async Task RunScriptAsync(byte[] inMemoryAssembly, byte[] inMemorySymbolStore, TextWriter outputTextWriter, TextWriter errorTextWriter, string newCurrentDirectory, CancellationToken cancellationToken)
        {
            cancellationToken.Register(StopScriptCore);
            await Task.Run(() => RunScriptCore(inMemoryAssembly, inMemorySymbolStore, outputTextWriter, errorTextWriter, newCurrentDirectory), cancellationToken);
        }

        private void RunScriptCore(byte[] inMemoryAssembly, byte[] inMemorySymbolStore, TextWriter outputTextWriter, TextWriter errorTextWriter, string newCurrentDirectory)
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
                if (newCurrentDirectory != null)
                {
                    oldCurrentDirectory = Directory.GetCurrentDirectory();
                    Directory.SetCurrentDirectory(newCurrentDirectory);
                }
            }

            try
            {
                scriptRun?.Run();
            }
            catch (AppDomainUnloadedException)
            {
            }
            finally
            {
                if (newCurrentDirectory != null)
                {
                    Directory.SetCurrentDirectory(oldCurrentDirectory);
                }
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
                    if (oldCurrentDirectory != null)
                    {
                        Directory.SetCurrentDirectory(oldCurrentDirectory);
                        oldCurrentDirectory = null;
                    }
                }
            }
        }
    }
}
