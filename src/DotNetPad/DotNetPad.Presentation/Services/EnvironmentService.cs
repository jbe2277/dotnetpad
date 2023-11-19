using System.ComponentModel.Composition;
using Waf.DotNetPad.Applications.Services;

namespace Waf.DotNetPad.Presentation.Services;

[Export(typeof(IEnvironmentService))]
internal sealed class EnvironmentService : IEnvironmentService
{
    private readonly Lazy<IReadOnlyList<string>> filesToLoad;

    public EnvironmentService()
    {
        filesToLoad = new(() => Environment.GetCommandLineArgs().Skip(1).ToArray());
    }

    public IReadOnlyList<string> FilesToLoad => filesToLoad.Value;
}
