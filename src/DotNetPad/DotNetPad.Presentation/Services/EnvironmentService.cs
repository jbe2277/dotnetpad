using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.IO;
using System.Waf.Applications;
using Waf.DotNetPad.Applications.Services;

namespace Waf.DotNetPad.Presentation.Services
{
    [Export(typeof(IEnvironmentService))]
    internal class EnvironmentService : IEnvironmentService
    {
        private readonly Lazy<string> profilePath;
        private readonly Lazy<string> appSettingsPath;
        private readonly Lazy<IReadOnlyList<string>> filesToLoad;


        public EnvironmentService()
        {
            this.profilePath = new Lazy<string>(() =>
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), ApplicationInfo.Company, ApplicationInfo.ProductName, "ProfileOptimization"));
            this.appSettingsPath = new Lazy<string>(() =>
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), ApplicationInfo.Company, ApplicationInfo.ProductName, "Settings"));
            this.filesToLoad = new Lazy<IReadOnlyList<string>>(() => Environment.GetCommandLineArgs().Skip(1).ToArray());
        }


        public string ProfilePath { get { return profilePath.Value; } }

        public string AppSettingsPath { get { return appSettingsPath.Value; } }

        public IReadOnlyList<string> FilesToLoad { get { return filesToLoad.Value; } }
    }
}
