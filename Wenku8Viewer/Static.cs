using Avalonia.Platform.Storage;
using Wenku8Viewer.Models;

namespace Wenku8Viewer
{
    public static class Static
    {
        public static Settings? Settings { get; set; }
        public static IStorageProvider? StorageProvider { get; set; }
    }
}
