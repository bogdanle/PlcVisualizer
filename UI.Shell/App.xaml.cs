using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using PlcVisualizer.Interfaces;
using PlcVisualizer.Services;
using Prism.Ioc;
using UI.Infrastructure.Interfaces;
using UI.Infrastructure.Services;
using UI.Shell.Views;

namespace UI.Shell
{
    /// <summary>
    /// Interaction logic for App.xaml.
    /// </summary>
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register(typeof(IPlcService), typeof(MockPlcService));
            containerRegistry.Register(typeof(IMessageBoxService), typeof(MessageBoxService));
            containerRegistry.Register(typeof(IFileDialogService), typeof(FileDialogService));
            containerRegistry.Register(typeof(IErrorDialogService), typeof(ErrorDialogService));
            containerRegistry.Register(typeof(ILogger), typeof(Logger));
        }
    }
}
