using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using AngleSharp;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Wenku8Viewer.Models;

namespace Wenku8Viewer.ViewModels;

public class MainViewModel : ViewModelBase, IRoutableViewModel
{
    private bool _isFirstLoad = true;

    public MainViewModel(IScreen screen, IBrowsingContext context)
    {
        HostScreen = screen;
        _browsingContext = context;
        NavigateToNovelDetailsCommand = ReactiveCommand.Create<Novel>(NavigateToNovelDetails);
    }

    private IBrowsingContext _browsingContext;
    [Reactive] public ObservableCollection<Novel>? TodaysHot { get; set; }
    [Reactive] public ObservableCollection<Novel>? MonthlyHot { get; set; }
    [Reactive] public string? Username { get; set; }

    public string? UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);
    public IScreen HostScreen { get; }
    public ReactiveCommand<Novel, Unit> NavigateToNovelDetailsCommand { get; set; }

    public async void OnLoaded()
    {
        if (!_isFirstLoad)
            return;
        var indexDocument = await _browsingContext.OpenAsync("https://www.wenku8.net/index.php");
        Username =
            indexDocument.QuerySelector("div.main.m_top div.fl")?.TextContent[10..^7]
            ?? string.Empty;
        var rightBlocks = indexDocument.QuerySelectorAll("div#right .block .blockcontent ul.ultop");
        TodaysHot = new(
            rightBlocks[0]
                .Children.Select(x =>
                {
                    var anchor = x.QuerySelector("a");
                    return new Novel(
                        anchor?.Attributes["title"]?.Value,
                        anchor?.Attributes["href"]?.Value
                    );
                })
        );

        MonthlyHot = new(
            rightBlocks[1]
                .Children.Select(x =>
                {
                    var anchor = x.QuerySelector("a");
                    return new Novel(
                        anchor?.Attributes["title"]?.Value,
                        anchor?.Attributes["href"]?.Value
                    );
                })
        );
        _isFirstLoad = false;
    }

    public void NavigateToNovelDetails(Novel novel)
    {
        HostScreen.Router.Navigate.Execute(
            new NovelDetailsViewModel(HostScreen, novel, _browsingContext)
        );
    }

    public void NavigateToSearch()
    {
        HostScreen.Router.Navigate.Execute(new SearchViewModel(HostScreen, _browsingContext));
    }
}
