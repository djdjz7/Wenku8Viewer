using AngleSharp;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using Wenku8Viewer.Models;

namespace Wenku8Viewer.ViewModels;

public class MainViewModel : ViewModelBase, IRoutableViewModel
{
    private bool isFirstLoad = true;
    public MainViewModel(IScreen screen, IBrowsingContext context)
    {
        HostScreen = screen;
        browsingContext = context;
        NavigateToNovelDetailsCommand = ReactiveCommand.Create<Novel>(NavigateToNovelDetails);
    }
    private IBrowsingContext browsingContext;
    private ObservableCollection<Novel> todaysHot = new();
    private ObservableCollection<Novel> monthlyHot = new();
    private string username = string.Empty;
    public ObservableCollection<Novel> TodaysHot
    {
        get => todaysHot;
        set => this.RaiseAndSetIfChanged(ref todaysHot, value);
    }
    public ObservableCollection<Novel> MonthlyHot
    {
        get => monthlyHot;
        set => this.RaiseAndSetIfChanged(ref monthlyHot, value);
    }
    public string Username
    {
        get => username;
        set => this.RaiseAndSetIfChanged(ref username, value);
    }

    public string? UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);
    public IScreen HostScreen { get; }
    public ReactiveCommand<Novel, Unit> NavigateToNovelDetailsCommand { get; set; }
    public async void OnLoaded()
    {
        if (!isFirstLoad)
            return;
        var indexDocument = await browsingContext.OpenAsync("https://www.wenku8.net/index.php");
        Username = indexDocument.QuerySelector("div.main.m_top div.fl")?.TextContent[10..^7] ?? string.Empty;
        var rightBlocks = indexDocument.QuerySelectorAll("div#right .block .blockcontent ul.ultop");
        TodaysHot = new(rightBlocks[0].Children.Select(x =>
        {
            var anchor = x.QuerySelector("a");
            return new Novel(anchor?.Attributes["title"]?.Value,
                             anchor?.Attributes["href"]?.Value);
        }));

        MonthlyHot = new(rightBlocks[1].Children.Select(x =>
        {
            var anchor = x.QuerySelector("a");
            return new Novel(anchor?.Attributes["title"]?.Value,
                             anchor?.Attributes["href"]?.Value);
        }));
        isFirstLoad = false;
    }

    public void NavigateToNovelDetails(Novel novel)
    {
        HostScreen.Router.Navigate.Execute(new NovelDetailsViewModel(HostScreen, novel, browsingContext));
    }

    public void NavigateToSearch()
    {
        HostScreen.Router.Navigate.Execute(new SearchViewModel(HostScreen, browsingContext));
    }
}
