using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using AngleSharp;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Wenku8Viewer.Models;
using Wenku8Viewer.Utils;

namespace Wenku8Viewer.ViewModels;

public class NovelDetailsViewModel : ViewModelBase, IRoutableViewModel
{
    public NovelDetailsViewModel(IScreen screen, Novel novel, IBrowsingContext context)
    {
        HostScreen = screen;
        CurrentNovel = novel;
        _browsingContext = context;
        OpenChapterReaderCommand = ReactiveCommand.Create<string>(OpenChapterReader);
        DownloadCommand = ReactiveCommand.CreateFromTask(
            Download,
            this.WhenAnyValue(
                x => x.NovelVolumeList,
                x =>
                {
                    return x != null && x.Count > 0;
                }
            )
        );
        FavoriteCommand = ReactiveCommand.Create(Favorite);
    }

    private IBrowsingContext _browsingContext;
    public string? UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);
    public IScreen HostScreen { get; }
    public ReactiveCommand<string, Unit> OpenChapterReaderCommand { get; set; }
    public ReactiveCommand<Unit, Unit> DownloadCommand { get; set; }
    public ReactiveCommand<Unit, Unit> FavoriteCommand { get; set; }

    [Reactive]
    public Novel CurrentNovel { get; set; }

    [Reactive]
    public List<Volume>? NovelVolumeList { get; set; }

    public async void OnLoaded()
    {
        CurrentNovel = await NovelUtils.GetNovelDetails(_browsingContext, CurrentNovel.NovelID);
        NovelVolumeList = await NovelUtils.GetVolumesAsync(_browsingContext, CurrentNovel.NovelID);
    }

    public void OpenChapterReader(string chapterUrl)
    {
        var novelID = CurrentNovel.NovelID;
        HostScreen.Router.Navigate.Execute(
            new ReaderViewModel(
                HostScreen,
                _browsingContext,
                $"https://www.wenku8.net/novel/{novelID / 1000}/{novelID}/{chapterUrl}",
                NovelVolumeList ?? []
            )
        );
    }

    public async Task Download() { }

    public void Favorite() { }
}
