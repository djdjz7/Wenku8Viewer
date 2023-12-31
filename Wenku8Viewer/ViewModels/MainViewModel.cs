using AngleSharp;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Wenku8Viewer.Models;
using Wenku8Viewer.Utils;

namespace Wenku8Viewer.ViewModels;

public class MainViewModel : ViewModelBase, IRoutableViewModel
{
    public MainViewModel(IScreen screen, IBrowsingContext context)
    {
        HostScreen = screen;
        browsingContext = context;
    }
    private IBrowsingContext browsingContext;
    private ObservableCollection<Novel> todaysHot = new ObservableCollection<Novel>();
    private bool isLoggedIn = false;
    public bool IsLoggedIn
    {
        get => isLoggedIn;
        set => this.RaiseAndSetIfChanged(ref isLoggedIn, value);
    }
    public ObservableCollection<Novel> TodaysHot
    {
        get => todaysHot;
        set => this.RaiseAndSetIfChanged(ref todaysHot, value);
    }

    public string? UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);
    public IScreen HostScreen { get; }
    public async void OnLoaded()
    {
        var indexDocument = await browsingContext.OpenAsync("https://www.wenku8.net/index.php");
        var rightBlocks = indexDocument.QuerySelectorAll("div#right .block .blockcontent ul.ultop");
        TodaysHot = new(rightBlocks[0].Children.Select(x =>
        {
            var anchor = x.QuerySelector("a");
            return new Novel(anchor.Attributes["title"].Value,
                             anchor.Attributes["href"].Value);
        }));

        /*
        TodaysHot = new(indexDocument.QuerySelectorAll("div#right .block .blockcontent ul.ultop li a").Select(x =>
        {
            return new Novel
            {
                NovelName = x.Attributes["title"].Value,
                NovelID = NovelUtils.ExtractNovelIDFromUrl(x.Attributes["href"].Value)
            };
        }));*/
    }
}
