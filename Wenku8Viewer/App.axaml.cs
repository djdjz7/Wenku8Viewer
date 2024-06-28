using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using System.IO;
using System.Text.Json;
using Wenku8Viewer.Models;
using Wenku8Viewer.Utils;
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
            var mainWindow = new MainWindow { DataContext = _mainWindowViewModel };
            mainWindow.Loaded += MainLoaded;
            desktop.MainWindow = mainWindow;
            Static.StorageProvider = TopLevel.GetTopLevel(mainWindow)?.StorageProvider;
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            var mainView = new MainViewPlatform { DataContext = _mainWindowViewModel };
            singleViewPlatform.MainView = mainView;
            mainView.Loaded += MainLoaded;
        }
        
        base.OnFrameworkInitializationCompleted();
    }

    public void MainLoaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(sender as Visual);
        if (topLevel is not null)
        {
            Static.StorageProvider = topLevel.StorageProvider;
            topLevel.BackRequested += (_, e) =>
            {
                _mainWindowViewModel?.NavigateBackCommand.Execute();
                e.Handled = true;
            };
        }
        try
        {
            var path = StorageHelper.GetSettingsPath();
            if (path is not null)
            {

                var configContent = File.ReadAllText(path);
                var config = JsonSerializer.Deserialize<Settings>(configContent);
                if (config is not null)
                    Static._settings = config;
                else
                    Static._settings = new Settings();
            }
            else
                Static._settings = new Settings();
        }
        catch
        {
            Static._settings = new Settings();
        }
    }
}
