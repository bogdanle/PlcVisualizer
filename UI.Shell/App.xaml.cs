using System;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlcVisualizer.Interfaces;
using PlcVisualizer.Services;
using PlcVisualizer.ViewModels;
using PlcVisualizer.Views;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using UI.Infrastructure;
using UI.Infrastructure.Interfaces;
using UI.Infrastructure.Services;
using UI.Shell.ViewModels;
using UI.Shell.Views;
using static Serilog.Log;

namespace UI.Shell;

public partial class App : Application
{
    public App()
    {
        Logger = CreateLogger();

        AppDomain.CurrentDomain.UnhandledException += (s, e) =>
        {
            Logger.Fatal(e.ExceptionObject as Exception, "An unhandled exception occurred.");
            Logger.Fatal("Application will be terminated.");
        };

        Host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder().
            UseContentRoot(AppContext.BaseDirectory).
            UseSerilog((ctx, config) => config.ReadFrom.Configuration(ctx.Configuration).WriteTo.Logger(Logger)).
            ConfigureServices((context, services) => ConfigureServices(services, context)).Build(); 

        AppServices.Provider = Host.Services;
    }

    public IHost Host { get; }

    public static T GetService<T>()
        where T : class
    {
        return AppServices.Provider.GetService(typeof(T)) is not T service ? 
            throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.") : service;
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        FrameworkElement.LanguageProperty.OverrideMetadata(
            typeof(FrameworkElement),
            new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

        var shell = GetService<MainWindow>();
        shell.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        if (Host is IDisposable disposable)
        {
            disposable.Dispose();
        }

        base.OnExit(e);
    }

    private static Serilog.ILogger CreateLogger()
    {
        return new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .Enrich.FromLogContext()
            .WriteTo.Console(
                outputTemplate: "{Timestamp:HH:mm:ss.fff} {Level:w3} {SourceContext}{NewLine}             {Message:lj}{NewLine}{Exception}",
                theme: AnsiConsoleTheme.Code)
            .WriteTo.Debug()
            .CreateLogger();
    }

    private static void ConfigureServices(IServiceCollection services, HostBuilderContext context)
    {
        services.AddSingleton<IMessenger>(_ => WeakReferenceMessenger.Default);

        services.AddSingleton<IPlcService, MockPlcService>();
        services.AddSingleton<IMessageBoxService, MessageBoxService>();
        services.AddSingleton<IFileDialogService, FileDialogService>();
        services.AddSingleton<IErrorDialogService, ErrorDialogService>();

        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<HamburgerMenuViewModel>();
        services.AddSingleton<TopBarViewModel>();
        services.AddSingleton<PlcViewModel>();

        services.AddSingleton<MainWindow>();
        services.AddSingleton<HamburgerMenu>();
        services.AddSingleton<TopBar>();
        services.AddSingleton<ToolbarStrip>();
        services.AddSingleton<NotificationCenter>();
        services.AddSingleton<PlcView>();
        services.AddSingleton<ToolbarStripContent>();
    }
}
