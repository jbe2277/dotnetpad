using Autofac;
using System.Waf.Applications.Services;
using System.Waf.Presentation.Services;
using Waf.DotNetPad.Applications.Services;
using Waf.DotNetPad.Applications.Views;
using Waf.DotNetPad.Presentation.Services;
using Waf.DotNetPad.Presentation.Views;

namespace Waf.DotNetPad.Presentation;

public class PresentationModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<FileDialogService>().As<IFileDialogService>().AsSelf().SingleInstance();
        builder.RegisterType<MessageService>().As<IMessageService>().SingleInstance();
        builder.RegisterType<SettingsService>().As<ISettingsService>().SingleInstance();

        builder.RegisterType<ClipboardService>().As<IClipboardService>().SingleInstance();
        builder.RegisterType<EnvironmentService>().As<IEnvironmentService>().SingleInstance();

        builder.RegisterType<CodeEditorView>().As<ICodeEditorView>();
        builder.RegisterType<ErrorListView>().As<IErrorListView>().SingleInstance();
        builder.RegisterType<InfoWindow>().As<IInfoView>();
        builder.RegisterType<OutputView>().As<IOutputView>().SingleInstance();
        builder.RegisterType<SaveChangesWindow>().As<ISaveChangesView>();
        builder.RegisterType<ShellWindow>().As<IShellView>().SingleInstance();
    }
}
