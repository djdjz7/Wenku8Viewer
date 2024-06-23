using AngleSharp;
using ReactiveUI;
using System;
using System.Reactive;
using System.Threading.Tasks;
using Wenku8Viewer.Utils;

namespace Wenku8Viewer.ViewModels;

public class ReaderViewModel : ViewModelBase, IRoutableViewModel
{
    public ReaderViewModel(IScreen screen, IBrowsingContext browsingContext, string chapterUrl)
    {
        HostScreen = screen;
        _browsingContext = browsingContext;
        _chapterUrl = new Uri(chapterUrl);
        SwitchPreviousCommand = ReactiveCommand.CreateFromTask(
            async () =>
            {
                await SwitchChapter(PreviousUrl!);
            },
            this.WhenAnyValue(x => x.PreviousUrl, (x) => !string.IsNullOrEmpty(x))
        );
        SwitchNextCommand = ReactiveCommand.CreateFromTask(
            async () =>
            {
                await SwitchChapter(NextUrl!);
            },
            this.WhenAnyValue(x => x.NextUrl, (x) => !string.IsNullOrEmpty(x))
        );
    }

    public IScreen HostScreen { get; }
    private IBrowsingContext _browsingContext;
    private Uri _chapterUrl = null!;
    private string? _chapterContent;
    private string? _chapterTitle;
    private string? _previousUrl;
    private string? _nextUrl;
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
    }

    public async Task SwitchChapter(string url)
    {
        _chapterUrl = new Uri(_chapterUrl, url);
        await OnLoaded();
    }
}
