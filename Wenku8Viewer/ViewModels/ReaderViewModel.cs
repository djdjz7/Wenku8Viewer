using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using AngleSharp;
using ReactiveUI;
using Wenku8Viewer.Models;
using Wenku8Viewer.Utils;

namespace Wenku8Viewer.ViewModels;

public class ReaderViewModel : ViewModelBase, IRoutableViewModel
{
    public ReaderViewModel(IScreen screen, IBrowsingContext browsingContext, string chapterUrl)
    {
        HostScreen = screen;
        _browsingContext = browsingContext;
        _chapterUrl = chapterUrl;
    }

    public IScreen HostScreen { get; }
    private IBrowsingContext _browsingContext;
    private string _chapterUrl = null!;
    private string? _chapterContent;
    private string? _chapterTitle;
    public string? ChapterContent
    {
        get => _chapterContent;
        set => this.RaiseAndSetIfChanged(ref _chapterContent, value);
    }
    public string? ChapterTitle
    {
        get => _chapterTitle;
        set => this.RaiseAndSetIfChanged(ref _chapterTitle, value);
    }
    public string? UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);
    public async void OnLoaded()
    {
        var document = await _browsingContext.OpenAsync(_chapterUrl);
        var contentDiv = document.QuerySelector("#content");
        var textContent = contentDiv?.TextContent ?? string.Empty;
        ChapterContent = NovelUtils.GetNovelContentReplaceRegex().Replace(textContent, string.Empty);
        var titleDiv = document.QuerySelector("#title");
        ChapterTitle = titleDiv?.TextContent ?? string.Empty;
    }
}
