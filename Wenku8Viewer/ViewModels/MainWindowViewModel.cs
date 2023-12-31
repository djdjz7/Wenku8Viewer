using ReactiveUI;
using System.Xml.Linq;
using System;
using System.Reactive;

namespace Wenku8Viewer.ViewModels;

public class MainWindowViewModel : ViewModelBase, IScreen
{
    public MainWindowViewModel()
    {
        GoNext = ReactiveCommand.CreateFromObservable(
                () => Router.Navigate.Execute(new LoginViewModel(this))
            );
    }

    public RoutingState Router { get; } = new RoutingState();
    public ReactiveCommand<Unit, IRoutableViewModel> GoNext { get; }
    public ReactiveCommand<Unit, IRoutableViewModel> GoBack => Router.NavigateBack;
    public void OnStartup()
    {
        Router.Navigate.Execute(new LoginViewModel(this));
    }
}
