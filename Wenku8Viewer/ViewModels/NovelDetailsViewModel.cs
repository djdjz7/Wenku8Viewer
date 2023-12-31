using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wenku8Viewer.Models;

namespace Wenku8Viewer.ViewModels;

public class NovelDetailsViewModel: ViewModelBase, IRoutableViewModel
{
    public NovelDetailsViewModel(IScreen screen, Novel novel)
    {
        HostScreen = screen;
        CurrentNovel = novel;
    }
    private Novel currentNovel;
    public Novel CurrentNovel
    {
        get => currentNovel;
        set => this.RaiseAndSetIfChanged(ref currentNovel, value);
    }
    public string? UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);
    public IScreen HostScreen { get; }
}
