using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wenku8Viewer.Utils
{
    public static class NovelUtils
    {
        public static int ExtractNovelIDFromUrl(string url)
        {
            return int.Parse(url.Split('/', StringSplitOptions.RemoveEmptyEntries)[1][0..^4]);
        }
    }
}
