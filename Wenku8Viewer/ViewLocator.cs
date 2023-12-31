using ReactiveUI;
using System;
using System.Threading;
using Wenku8Viewer.ViewModels;
using Wenku8Viewer.Views;

namespace Wenku8Viewer
{
    public class ViewLocator : ReactiveUI.IViewLocator
    {
        public IViewFor ResolveView<T>(T viewModel, string contract = null) => viewModel switch
        {
            LoginViewModel context => new LoginView { DataContext = context },
            MainViewModel context => new MainView { DataContext = context },
            NovelDetailsViewModel context => new NovelDetailsView { DataContext = context },
            _ => throw new ArgumentOutOfRangeException(nameof(viewModel))
        };
    }
}
