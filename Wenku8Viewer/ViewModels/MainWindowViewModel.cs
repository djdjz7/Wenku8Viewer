using ReactiveUI;
using System.Xml.Linq;
using System;
using System.Reactive;

namespace Wenku8Viewer.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel()
    {
        NavigateToCommand = ReactiveCommand.Create<string>(NavigateTo);
    }
    private ViewModelBase contentViewModel = new MainViewModel();
    public ReactiveCommand<string, Unit> NavigateToCommand { get; }
    public ViewModelBase ContentViewModel
    {
        get => contentViewModel;
        set => this.RaiseAndSetIfChanged(ref contentViewModel, value);
    }
    public void NavigateTo(string targetViewModelName)
    {
        var type = Type.GetType(targetViewModelName);
        if(type != null)
        {
            ContentViewModel = Activator.CreateInstance(type) as ViewModelBase;
        }

        ContentViewModel = new MainViewModel();
    }
}
