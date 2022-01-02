using System.ComponentModel.Composition;
using System.IO;
using System.Waf.Applications;
using Waf.DotNetPad.Applications.Services;

namespace Waf.DotNetPad.Presentation.Services
{
    [Export(typeof(IEnvironmentService))]
    internal class EnvironmentService : IEnvironmentService
    {
        private readonly Lazy<IReadOnlyList<string>> filesToLoad;

        public EnvironmentService()
        {
            filesToLoad = new Lazy<IReadOnlyList<string>>(() => Environment.GetCommandLineArgs().Skip(1).ToArray());
        }

        public static string ProfilePath { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), ApplicationInfo.Company, ApplicationInfo.ProductName, "ProfileOptimization");

        public IReadOnlyList<string> FilesToLoad => filesToLoad.Value;
    }
}
