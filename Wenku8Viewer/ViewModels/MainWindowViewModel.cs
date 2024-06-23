using ReactiveUI;
using System.Reactive;

namespace Wenku8Viewer.ViewModels;

public class MainWindowViewModel : ViewModelBase, IScreen
{
    public MainWindowViewModel() { }

    public RoutingState Router { get; } = new RoutingState();
    public ReactiveCommand<Unit, IRoutableViewModel> NavigateBack => Router.NavigateBack;

    public void OnStartup()
    {
        Router.Navigate.Execute(new LoginViewModel(this));
    }
}
