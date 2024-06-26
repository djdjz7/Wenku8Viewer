﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Web;
using AngleSharp;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Wenku8Viewer.Models;
using Wenku8Viewer.Utils;

namespace Wenku8Viewer.ViewModels;

public class SearchViewModel : ViewModelBase, IRoutableViewModel
{
    public SearchViewModel(IScreen screen, IBrowsingContext browsingContext)
    {
        HostScreen = screen;
        _browsingContext = browsingContext;
        SearchCommand = ReactiveCommand.Create(
            Search,
            this.WhenAnyValue(
                x => x.SearchMethod,
                x => x.SearchContent,
                (method, content) =>
                    (method == 0 || method == 1) && !string.IsNullOrWhiteSpace(content)
            )
        );
        NavigateToNovelDetailsCommand = ReactiveCommand.Create<Novel>(NavigateToNovelDetails);
    }

    private Encoding _gb2312 = Encoding.GetEncoding("gb2312");
    public string? UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);
    public IScreen HostScreen { get; }
    private IBrowsingContext _browsingContext;
    [Reactive] public string? SearchContent { get; set; }
    [Reactive] public int SearchMethod { get; set; }
    [Reactive] public ObservableCollection<Novel> SearchResults { get; set; }
    public ReactiveCommand<Unit, Unit> SearchCommand { get; }
    public ReactiveCommand<Novel, Unit> NavigateToNovelDetailsCommand { get; set; }

    public async void Search()
    {
        var searchPageDocument = await _browsingContext.OpenAsync(
            "https://www.wenku8.net/modules/article/search.php?"
                + $"searchtype={(SearchMethod == 0 ? "articlename" : "author")}"
                + $"&searchkey={HttpUtility.UrlEncode(SearchContent, _gb2312)}"
        );
        var allResults = searchPageDocument.QuerySelectorAll(
            "div#centerm div#content table.grid tbody tr td > div"
        );
        SearchResults = new(
            allResults.Select(x =>
            {
                var href = x.QuerySelector("div a")?.Attributes["href"]?.Value;
                var name = x.QuerySelector("div a")?.Attributes["title"]?.Value;
                var authorWithLib = x.QuerySelector("div p")?.TextContent;
                var author = authorWithLib?.Split('/')[0][3..];
                var tags = x.QuerySelector("div p:nth-child(4)")?.TextContent[5..];
                var description = x.QuerySelector("div p:nth-child(5)")?.TextContent;
                var otherData = x.QuerySelector("div p:nth-child(3)")?.TextContent.Split('/');
                DateOnly lastUpdate = new DateOnly();
                var status = string.Empty;
                if (otherData?.Length >= 3)
                {
                    lastUpdate = DateOnly.Parse(otherData[0][3..]);
                    status = otherData[2];
                }
                return new Novel(name, NovelUtils.ExtractNovelIDFromUrl(href))
                {
                    Author = author,
                    NovelTags = tags,
                    NovelDescription = description,
                    LastUpdate = lastUpdate,
                    NovelStatus = status,
                };
            })
        );
    }

    public void NavigateToNovelDetails(Novel novel)
    {
        HostScreen.Router.Navigate.Execute(
            new NovelDetailsViewModel(HostScreen, novel, _browsingContext)
        );
    }
}
