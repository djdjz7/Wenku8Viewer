using System;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Wenku8Viewer.Utils;

namespace Wenku8Viewer.Models
{
    public class Novel
    {
        public int NovelID { get; init; }
        public string? NovelName { get; init; }
        public string CoverUrl
        {
            get => $"https://img.wenku8.com/image/{NovelID / 1000}/{NovelID}/{NovelID}s.jpg";
        }
        public string? Author { get; set; }
        public string? NovelStatus { get; set; }
        public string? RecentChapter { get; set; }
        public string? NovelTags { get; set; }
        public string? NovelDescription { get; set; }
        public DateOnly? LastUpdate { get; set; }
        public Task<Bitmap?>? ImageFromWebsite { get; }
        public Volume[]? Volumes { get; set; }

        public Novel(string? novelName, string? href)
        {
            if (novelName is null || href is null)
            {
                ImageFromWebsite = new Task<Bitmap?>(() =>
                {
                    return null;
                });
            }
            NovelName = novelName;
            NovelID = NovelUtils.ExtractNovelIDFromUrl(href!);
            ImageFromWebsite = ImageHelper.LoadFromWeb(new Uri(CoverUrl));
        }

        public Novel(string? novelName, int novelID)
        {
            if (novelName is null)
            {
                ImageFromWebsite = new Task<Bitmap?>(() =>
                {
                    return null;
                });
            }
            NovelName = novelName;
            NovelID = novelID;
            ImageFromWebsite = ImageHelper.LoadFromWeb(new Uri(CoverUrl));
        }

        public Novel() { }
    }
}
