using ReactiveUI;

namespace Wenku8Viewer.ViewModels;

public class MainViewModel : ViewModelBase
{
    private bool isLoggedIn = false;
    public bool IsLoggedIn
    {
        get => isLoggedIn;
        set => this.RaiseAndSetIfChanged(ref isLoggedIn, value);
    }
}
