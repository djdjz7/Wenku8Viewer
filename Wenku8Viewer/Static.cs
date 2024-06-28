using System;
using System.IO;
using System.Text.Json;
using Avalonia.Platform.Storage;
using Wenku8Viewer.Models;
using Wenku8Viewer.Utils;

namespace Wenku8Viewer
{
    public static class Static
    {
        internal static Settings? _settings;
        public static Settings? Settings
        {
            get => _settings;
            set
            {
                _settings = value;
                var path = StorageHelper.GetSettingsPath();
                if (path is not null && value is not null)
                    File.WriteAllText(
                        path,
                        JsonSerializer.Serialize(
                            Static.Settings,
                            new JsonSerializerOptions { WriteIndented = true }
                        )
                    );
            }
        }
        public static IStorageProvider? StorageProvider { get; set; }
    }
}
