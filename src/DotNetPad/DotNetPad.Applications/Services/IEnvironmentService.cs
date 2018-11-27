using System.Collections.Generic;

namespace Waf.DotNetPad.Applications.Services
{
    public interface IEnvironmentService
    {
        IReadOnlyList<string> FilesToLoad { get; }
    }
}
