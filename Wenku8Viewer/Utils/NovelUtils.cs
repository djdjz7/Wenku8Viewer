using AngleSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wenku8Viewer.Models;

namespace Wenku8Viewer.Utils
{
    public static class NovelUtils
    {
        public static int ExtractNovelIDFromUrl(string? url)
        {
            if (url is null)
                return 0;
            return int.Parse(url.Split('/', StringSplitOptions.RemoveEmptyEntries)[1][0..^4]);
        }
        public async static Task<Novel> GetNovelDetails(IBrowsingContext browsingContext, int novelID)
        {
            var novelPageDocument = await browsingContext.OpenAsync($"https://www.wenku8.net/book/{novelID}.htm");
            var novelName = novelPageDocument.QuerySelector("div#centerl div#content div table tbody tr td table tbody tr td span b")?.TextContent;
            var novelDetails = novelPageDocument.QuerySelector("div.main div#centerl div#content div table:nth-child(4) tbody tr td:nth-child(2)");
            var recentChapter = novelDetails?.QuerySelector("span:nth-child(8) a")?.TextContent;
            var novelDescription = novelDetails?.QuerySelector("span:nth-child(13)")?.TextContent;
            var novelTags = novelDetails?.QuerySelector("span:nth-child(1) b")?.TextContent;
            var author = novelPageDocument.QuerySelector("div#centerl div#content div table tbody tr:nth-child(2) td:nth-child(2)")?.TextContent;
            var status = novelPageDocument.QuerySelector("div#centerl div#content div table tbody tr:nth-child(2) td:nth-child(3)")?.TextContent;
            var lastUpdate = novelPageDocument.QuerySelector("div#centerl div#content div table tbody tr:nth-child(2) td:nth-child(4)")?.TextContent;

            return new Novel(novelName, novelID)
            {
                RecentChapter = recentChapter,
                NovelDescription = novelDescription,
                NovelTags = novelTags?[7..],
                Author = author?[5..],
                NovelStatus = status?[5..],
                LastUpdate = DateOnly.Parse(lastUpdate?[5..] ?? ""),
            };
        }
    }
}
