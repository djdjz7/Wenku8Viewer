using System;
using System.Collections.Generic;

namespace Wenku8Viewer.Models;

public class FavoriteNovelEntry
{
    public int NovelID { get; set; }
    public string? NovelName { get; set; }
    public string? Author { get; set; }
    public string? RecentChapter { get; set; }
    public DateOnly? FavoriteDate { get; set; }
}

public class ReaderSettings
{
    public int FontSize { get; set; } = 14;
    public int FontType { get; set; }
}

public class Settings
{
    public ReaderSettings ReaderSettings { get; set; } = new ReaderSettings();
    // public List<FavoriteNovelEntry>? FavoriteNovels { get; set; }
}
