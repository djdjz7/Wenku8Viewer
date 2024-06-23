using Avalonia.ReactiveUI;
using ReactiveUI;
using Wenku8Viewer.ViewModels;

namespace Wenku8Viewer.Views
{
    public partial class MainViewPlatform : ReactiveUserControl<MainWindowViewModel>
    {
        public MainViewPlatform()
        {
            this.WhenActivated(disposables => { });
            InitializeComponent();
        }
    }
}
