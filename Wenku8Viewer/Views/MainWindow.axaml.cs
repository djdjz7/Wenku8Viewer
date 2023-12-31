using Avalonia.Controls;
using Avalonia.ReactiveUI;
using ReactiveUI;
using Wenku8Viewer.ViewModels;

namespace Wenku8Viewer.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
    }

    private void Binding(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
    }
}
