using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Wenku8Viewer.Utils;
using static System.Net.WebRequestMethods;

namespace Wenku8Viewer.Models
{
    public class Novel
    {
        public int NovelID { get; init; }
        public string? NovelName { get; init; }
        public string CoverUrl { get => $"https://img.wenku8.com/image/{NovelID/1000}/{NovelID}/{NovelID}s.jpg"; }
        public string? Author { get; set; }
        public bool IsFinished { get; set; }
        public Task<Bitmap?> ImageFromWebsite { get; }
        public Novel (string novelName, string href)
        {
            NovelName = novelName;
            NovelID = NovelUtils.ExtractNovelIDFromUrl(href);
            ImageFromWebsite = ImageHelper.LoadFromWeb(new Uri(CoverUrl));
        }
    }
}