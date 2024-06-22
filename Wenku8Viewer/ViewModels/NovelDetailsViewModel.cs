using AngleSharp;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Wenku8Viewer.Models;
using Wenku8Viewer.Utils;

namespace Wenku8Viewer.ViewModels;

public class NovelDetailsViewModel: ViewModelBase, IRoutableViewModel
{
    public NovelDetailsViewModel(IScreen screen, Novel novel, IBrowsingContext context)
    {
        HostScreen = screen;
        CurrentNovel = novel;
        _browsingContext = context;
        OpenChapterReaderCommand = ReactiveCommand.Create<string>(OpenChapterReader);
    }
    private Novel _currentNovel = null!;
    private IBrowsingContext _browsingContext;
    public ReactiveCommand<string, Unit> OpenChapterReaderCommand { get; set; }
    public Novel CurrentNovel
    {
        get => _currentNovel;
        set => this.RaiseAndSetIfChanged(ref _currentNovel, value);
    }
    public string? UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);
    public IScreen HostScreen { get; }
    private List<Volume>? _novelVolumeList;
    public List<Volume> NovelVolumeList
    {
        get => _novelVolumeList??= new List<Volume>();
        set => this.RaiseAndSetIfChanged(ref _novelVolumeList, value);
    }
    public async void OnLoaded()
    {
        CurrentNovel = await NovelUtils.GetNovelDetails(_browsingContext, CurrentNovel.NovelID);
        NovelVolumeList = await NovelUtils.GetVolumesAsync(_browsingContext, CurrentNovel.NovelID);
    }

    public void OpenChapterReader(string chapterUrl)
    {
        var novelID = CurrentNovel.NovelID;
        HostScreen.Router.Navigate.Execute(new ReaderViewModel(HostScreen, _browsingContext, $"https://www.wenku8.net/novel/{novelID / 1000}/{novelID}/{chapterUrl}" ));
    }
}
