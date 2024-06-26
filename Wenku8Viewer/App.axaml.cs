using System.IO;
using System.Text.Json;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Wenku8Viewer.Models;
using Wenku8Viewer.ViewModels;
using Wenku8Viewer.Views;

namespace Wenku8Viewer;

public partial class App : Application
{
    public override void Initialize()
    {
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        AvaloniaXamlLoader.Load(this);
    }

    private MainWindowViewModel? _mainWindowViewModel = new MainWindowViewModel();

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Exit += Desktop_Exit;
            var mainWindow = new MainWindow { DataContext = _mainWindowViewModel };
            desktop.MainWindow = mainWindow;
            Static.StorageProvider = TopLevel.GetTopLevel(mainWindow)?.StorageProvider;
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            var mainView = new MainViewPlatform { DataContext = _mainWindowViewModel };
            singleViewPlatform.MainView = mainView;
            mainView.Loaded += (_, _) =>
            {
                var topLevel = TopLevel.GetTopLevel(mainView);
                if (topLevel is not null)
                {
                    Static.StorageProvider = topLevel.StorageProvider;
                    topLevel.BackRequested += (_, e) =>
                    {
                        _mainWindowViewModel?.NavigateBackCommand.Execute();
                        e.Handled = true;
                    };
                }
            };
        }
        try
        {
            var configContent = File.ReadAllText("Settings.json");
            var config = JsonSerializer.Deserialize<Settings>(configContent);
            if (config is not null)
                Static.Settings = config;
            else
                Static.Settings = new Settings();
        }
        catch
        {
            Static.Settings = new Settings();
        }
        base.OnFrameworkInitializationCompleted();
    }

    private void Desktop_Exit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        try
        {
            File.WriteAllText(
                "Settings.json",
                JsonSerializer.Serialize(
                    Static.Settings,
                    new JsonSerializerOptions { WriteIndented = true }
                )
            );
        }
        catch { }
    }
}
