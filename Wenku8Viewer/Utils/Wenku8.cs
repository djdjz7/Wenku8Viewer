using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using AngleSharp;
using Wenku8Viewer.Models;

namespace Wenku8Viewer.Utils
{
    internal static class Wenku8
    {
        private static Encoding _gb2312 = Encoding.GetEncoding("gb2312");

        public static async Task<IEnumerable<Novel>> Search(
            string? searchContent,
            int searchMethod,
            IBrowsingContext browsingContext
        )
        {
            if (searchContent is null)
                return [];
            var searchPageDocument = await browsingContext.OpenAsync(
                "https://www.wenku8.net/modules/article/search.php?"
                    + $"searchtype={(searchMethod == 0 ? "articlename" : "author")}"
                    + $"&searchkey={HttpUtility.UrlEncode(searchContent, _gb2312)}"
            );
            if (searchPageDocument.BaseUri.Contains("search.php"))
            {
                var allResults = searchPageDocument.QuerySelectorAll(
                    "div#centerm div#content table.grid tbody tr td > div"
                );
                return allResults.Select(x =>
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
                });
            }
            else
            {
                throw new NovelSearchRedirectException(NovelUtils.ExtractNovelIDFromUrl(searchPageDocument.BaseUri));
            }
        }
    }

    internal class NovelSearchRedirectException : Exception
    {
        public int NovelID { get; }
        public NovelSearchRedirectException(int novelID)
        {
            NovelID = novelID;
        }
    }

}
