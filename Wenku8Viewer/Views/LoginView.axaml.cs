using Avalonia.Controls;
using Avalonia.ReactiveUI;
using ReactiveUI;
using Wenku8Viewer.ViewModels;

namespace Wenku8Viewer.Views
{
    public partial class LoginView : ReactiveUserControl<LoginViewModel>
    {
        public LoginView()
        {
            this.WhenActivated(disposables => { });
            InitializeComponent();
        }
    }
}
