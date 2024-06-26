using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
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

    public string? UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);
    public IScreen HostScreen { get; }
    private IBrowsingContext _browsingContext;

    [Reactive]
    public string? SearchContent { get; set; }

    [Reactive]
    public int SearchMethod { get; set; }

    [Reactive]
    public ObservableCollection<Novel>? SearchResults { get; set; }
    public ReactiveCommand<Unit, Unit> SearchCommand { get; }
    public ReactiveCommand<Novel, Unit> NavigateToNovelDetailsCommand { get; set; }

    public async void Search()
    {
        try
        {
            SearchResults = new(await Wenku8.Search(SearchContent, SearchMethod, _browsingContext));
        }
        catch (NovelSearchRedirectException e)
        {
            await HostScreen.Router.Navigate.Execute(
                new NovelDetailsViewModel(
                    HostScreen,
                    new Novel() { NovelID = e.NovelID },
                    _browsingContext
                )
            );
        }
    }

    public void NavigateToNovelDetails(Novel novel)
    {
        HostScreen.Router.Navigate.Execute(
            new NovelDetailsViewModel(HostScreen, novel, _browsingContext)
        );
    }
}
