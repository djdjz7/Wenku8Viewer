using Avalonia.ReactiveUI;
using ReactiveUI;
using Wenku8Viewer.ViewModels;

namespace Wenku8Viewer.Views;

public partial class NovelDetailsView : ReactiveUserControl<NovelDetailsViewModel>
{
    public NovelDetailsView()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
    }
}
