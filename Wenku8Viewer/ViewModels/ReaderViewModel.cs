using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using AngleSharp;
using Avalonia;
using Avalonia.Media.Imaging;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
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
        Volumes = volumeList;
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
        GoToChapterCommand = ReactiveCommand.CreateFromTask<string>(GoToChapter);
        _chapters = volumeList.SelectMany(x => x.Chapters ?? Enumerable.Empty<Chapter>()).ToList();
        for (int i = 0; i < _chapters.Count; i++)
        {
            if (_chapters[i].Url is null)
                continue;
            if (chapterUrl.EndsWith(_chapters[i].Url!))
            {
                CurrentIndex = i;
                break;
            }
        }
    }

    public IScreen HostScreen { get; }
    private IBrowsingContext _browsingContext;
    public string? UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);

    private Uri _chapterUrl = null!;
    private int _currentIndex;
    private List<Chapter> _chapters;

    public Vector ScrollOffset { get; } = Vector.Zero;

    [Reactive]
    public string? ChapterContent { get; set; }

    [Reactive]
    public List<Task<Bitmap?>>? Illustrations { get; set; }

    [Reactive]
    public string? ChapterTitle { get; set; }

    [Reactive]
    public string? PreviousUrl { get; set; }

    [Reactive]
    public string? NextUrl { get; set; }

    [Reactive]
    public string? PreviousTitle { get; set; }

    [Reactive]
    public string? NextTitle { get; set; }

    [Reactive]
    public List<Volume> Volumes { get; set; }

    [Reactive]
    public int CurrentIndex { get; set; }
    public int FontType
    {
        get => Static.Settings!.ReaderSettings.FontType;
        set
        {
            if (Static.Settings!.ReaderSettings.FontType != value)
            {
                Static.Settings.ReaderSettings.FontType = value;
                this.RaisePropertyChanged(nameof(FontType));
                Static.Settings = Static.Settings;
            }
        }
    }
    public int FontSize
    {
        get => Static.Settings!.ReaderSettings.FontSize;
        set
        {
            if (Static.Settings!.ReaderSettings.FontSize != value)
            {
                Static.Settings.ReaderSettings.FontSize = value;
                this.RaisePropertyChanged(nameof(FontSize));
                Static.Settings = Static.Settings;
            }
        }
    }
    public ReactiveCommand<Unit, Unit> SwitchPreviousCommand { get; }
    public ReactiveCommand<Unit, Unit> SwitchNextCommand { get; }
    public ReactiveCommand<string, Unit> GoToChapterCommand { get; }

    public async Task OnLoaded()
    {
        var document = await _browsingContext.OpenAsync(_chapterUrl.ToString());
        var contentDiv = document.QuerySelector("#content");
        var textContent = contentDiv?.TextContent ?? string.Empty;
        ChapterContent = NovelUtils
            .GetNovelContentReplaceRegex()
            .Replace(textContent, string.Empty)
            .Trim();
        var titleDiv = document.QuerySelector("#title");
        ChapterTitle = titleDiv?.TextContent?.Trim() ?? string.Empty;

        var illustrationsElements = contentDiv?.QuerySelectorAll("div.divimage a img");
        if (illustrationsElements is not null)
        {
            Illustrations = illustrationsElements
                .Select(x =>
                {
                    var src = x.Attributes["src"]?.Value;
                    if (src is null)
                        return Task.FromResult<Bitmap?>(null);
                    return ImageHelper.LoadFromWeb(new Uri(src));
                })
                .ToList();
        }
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

        if (CurrentIndex == 0)
            PreviousTitle = string.Empty;
        else
            PreviousTitle = _chapters[CurrentIndex - 1].Title;
        if (CurrentIndex == _chapters.Count - 1)
            NextTitle = string.Empty;
        else
            NextTitle = _chapters[CurrentIndex + 1].Title;
        this.RaisePropertyChanged(nameof(ScrollOffset));
    }

    public async Task SwitchChapter(string url, bool isNext)
    {
        _chapterUrl = new Uri(_chapterUrl, url);
        CurrentIndex += isNext ? 1 : -1;
        await OnLoaded();
    }

    public async Task GoToChapter(string url)
    {
        _chapterUrl = new Uri(_chapterUrl, url);
        for (int i = 0; i < _chapters.Count; i++)
        {
            if (_chapters[i].Url is null)
                continue;
            if (url.EndsWith(_chapters[i].Url!))
            {
                CurrentIndex = i;
                break;
            }
        }
        await OnLoaded();
    }
}
