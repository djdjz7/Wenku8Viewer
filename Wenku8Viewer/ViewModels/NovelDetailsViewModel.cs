﻿using AngleSharp;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wenku8Viewer.Models;
using Wenku8Viewer.Utils;

namespace Wenku8Viewer.ViewModels;

public class NovelDetailsViewModel: ViewModelBase, IRoutableViewModel
{
    public NovelDetailsViewModel(IScreen screen, Novel novel, IBrowsingContext context)
    {
        HostScreen = screen;
        CurrentNovel = novel;
        browsingContext = context;
    }
    private Novel currentNovel;
    private IBrowsingContext browsingContext;
    public Novel CurrentNovel
    {
        get => currentNovel;
        set => this.RaiseAndSetIfChanged(ref currentNovel, value);
    }
    public string? UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);
    public IScreen HostScreen { get; }
    public async void OnLoaded()
    {
        CurrentNovel = await NovelUtils.GetNovelDetails(browsingContext, CurrentNovel.NovelID);
    }
}
