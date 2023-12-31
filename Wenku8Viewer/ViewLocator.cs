using Avalonia.Controls.Templates;
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wenku8Viewer.ViewModels;
using ReactiveUI;
using Wenku8Viewer.Views;

namespace Wenku8Viewer
{
    public class ViewLocator : ReactiveUI.IViewLocator
    {
        public IViewFor ResolveView<T>(T viewModel, string contract = null) => viewModel switch
        {
            LoginViewModel context => new LoginView { DataContext = context },
            MainViewModel context => new MainView { DataContext = context },
            _ => throw new ArgumentOutOfRangeException(nameof(viewModel))
        };
    }
}
