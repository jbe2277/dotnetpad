using Autofac;
using System.Waf.Applications;
using Waf.DotNetPad.Applications.Controllers;
using Waf.DotNetPad.Applications.Services;
using Waf.DotNetPad.Applications.ViewModels;

namespace Waf.DotNetPad.Applications;

public class ApplicationsModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<FileController>().AsSelf().SingleInstance();
        builder.RegisterType<ModuleController>().As<IModuleController>().SingleInstance();
        builder.RegisterType<WorkspaceController>().As<IWorkspaceService>().AsSelf().SingleInstance();

        builder.RegisterType<CodeEditorService>().As<ICodeEditorService>().AsSelf().SingleInstance();
        builder.RegisterType<CSharpSampleService>().AsSelf().SingleInstance();
        builder.RegisterType<FileService>().As<IFileService>().As<IDocumentService>().AsSelf().SingleInstance();
        builder.RegisterType<ShellService>().As<IShellService>().AsSelf().SingleInstance();
        builder.RegisterType<VisualBasicSampleService>().AsSelf().SingleInstance();

        builder.RegisterType<CodeEditorViewModel>().AsSelf();
        builder.RegisterType<ErrorListViewModel>().AsSelf().SingleInstance();
        builder.RegisterType<InfoViewModel>().AsSelf();
        builder.RegisterType<OutputViewModel>().AsSelf().SingleInstance();
        builder.RegisterType<SaveChangesViewModel>().AsSelf();
        builder.RegisterType<ShellViewModel>().AsSelf().SingleInstance();
    }
}