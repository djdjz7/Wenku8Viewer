using Avalonia.Controls;
using Avalonia.ReactiveUI;
using ReactiveUI;
using Wenku8Viewer.ViewModels;

namespace Wenku8Viewer.Views
{
    public partial class ReaderView : ReactiveUserControl<ReaderViewModel>
    {
        public ReaderView()
        {
            this.WhenActivated(disposables => { });
            InitializeComponent();
        }
    }
}
