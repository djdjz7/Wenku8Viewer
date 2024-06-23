﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using AngleSharp;
using Avalonia;
using ReactiveUI;
using Wenku8Viewer.Models;
using Wenku8Viewer.Utils;

namespace Wenku8Viewer.ViewModels;

public class ReaderViewModel : ViewModelBase, IRoutableViewModel
{
    public ReaderViewModel(
        IScreen screen,
        IBrowsingContext browsingContext,
        string chapterUrl,
        List<Volume> volumeList
    )
    {
        HostScreen = screen;
        _browsingContext = browsingContext;
        _chapterUrl = new Uri(chapterUrl);
        SwitchPreviousCommand = ReactiveCommand.CreateFromTask(
            async () =>
            {
                await SwitchChapter(PreviousUrl!, false);
            },
            this.WhenAnyValue(x => x.PreviousUrl, (x) => !string.IsNullOrEmpty(x))
        );
        SwitchNextCommand = ReactiveCommand.CreateFromTask(
            async () =>
            {
                await SwitchChapter(NextUrl!, true);
            },
            this.WhenAnyValue(x => x.NextUrl, (x) => !string.IsNullOrEmpty(x))
        );
        _chapters = volumeList.SelectMany(x => x.Chapters ?? Enumerable.Empty<Chapter>()).ToList();
        for (int i = 0; i < _chapters.Count; i++)
        {
            if (_chapters[i].Url is null)
                continue;
            if (chapterUrl.EndsWith(_chapters[i].Url!))
            {
                _currentIndex = i;
                break;
            }
        }
    }

    public IScreen HostScreen { get; }
    private IBrowsingContext _browsingContext;
    private Uri _chapterUrl = null!;
    private string? _chapterContent;
    private string? _chapterTitle;
    private string? _previousUrl;
    private string? _nextUrl;
    private string? _previousTitle;
    private string? _nextTitle;
    private int _currentIndex;
    private int _fontType;
    private int _fontSize = 14;
    private List<Chapter> _chapters;

    public Vector ScrollOffset { get; } = Vector.Zero;
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
    public string? PreviousUrl
    {
        get => _previousUrl;
        set => this.RaiseAndSetIfChanged(ref _previousUrl, value);
    }
    public string? NextUrl
    {
        get => _nextUrl;
        set => this.RaiseAndSetIfChanged(ref _nextUrl, value);
    }
    public string? PreviousTitle
    {
        get => _previousTitle;
        set => this.RaiseAndSetIfChanged(ref _previousTitle, value);
    }
    public string? NextTitle
    {
        get => _nextTitle;
        set => this.RaiseAndSetIfChanged(ref _nextTitle, value);
    }
    public int FontType
    {
        get => _fontType;
        set => this.RaiseAndSetIfChanged(ref _fontType, value);
    }
    public int FontSize
    {
        get => _fontSize;
        set => this.RaiseAndSetIfChanged(ref _fontSize, value);
    }
    public string? UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);
    public ReactiveCommand<Unit, Unit> SwitchPreviousCommand { get; }
    public ReactiveCommand<Unit, Unit> SwitchNextCommand { get; }

    public async Task OnLoaded()
    {
        var document = await _browsingContext.OpenAsync(_chapterUrl.ToString());
        var contentDiv = document.QuerySelector("#content");
        var textContent = contentDiv?.TextContent ?? string.Empty;
        ChapterContent = NovelUtils
            .GetNovelContentReplaceRegex()
            .Replace(textContent, string.Empty);
        var titleDiv = document.QuerySelector("#title");
        ChapterTitle = titleDiv?.TextContent ?? string.Empty;
        var footTextDiv = document.QuerySelector("#foottext");
        if (footTextDiv is null)
            return;
        var prevA = footTextDiv.QuerySelector("a:nth-child(3)");
        var nextA = footTextDiv.QuerySelector("a:nth-child(4)");
        var prevUrl = prevA?.Attributes["href"]?.Value ?? string.Empty;
        var nextUrl = nextA?.Attributes["href"]?.Value ?? string.Empty;
        if (prevUrl.Contains("index"))
            PreviousUrl = string.Empty;
        else
            PreviousUrl = prevUrl;
        if (nextUrl.Contains("last"))
            NextUrl = string.Empty;
        else
            NextUrl = nextUrl;
        
        if(_currentIndex == 0)
            PreviousTitle = string.Empty;
        else
            PreviousTitle = _chapters[_currentIndex - 1].Title;
        if(_currentIndex == _chapters.Count - 1)
            NextTitle = string.Empty;
        else
            NextTitle = _chapters[_currentIndex + 1].Title;
        this.RaisePropertyChanged(nameof(ScrollOffset));
    }

    public async Task SwitchChapter(string url, bool isNext)
    {
        _chapterUrl = new Uri(_chapterUrl, url);
        _currentIndex += isNext ? 1 : -1;
        await OnLoaded();
    }
}
