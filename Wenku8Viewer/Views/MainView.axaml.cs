using Avalonia.Controls;
using Avalonia.ReactiveUI;
using ReactiveUI;
using Wenku8Viewer.ViewModels;

namespace Wenku8Viewer.Views;

public partial class MainView : ReactiveUserControl<MainViewModel>
{
    public MainView()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
    }
}
