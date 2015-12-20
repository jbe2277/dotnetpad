using System.Collections.Generic;
namespace Waf.DotNetPad.Applications.Services
{
    public interface IEnvironmentService
    {
        string AppSettingsPath { get; }

        IReadOnlyList<string> FilesToLoad { get; }
    }
}
