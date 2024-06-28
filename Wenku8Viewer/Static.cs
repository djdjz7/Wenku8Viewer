using System;
using System.IO;
using System.Text.Json;
using Avalonia.Platform.Storage;
using Wenku8Viewer.Models;

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
                if (OperatingSystem.IsWindows())
                {
                    File.WriteAllText(
                        "Settings.json",
                        JsonSerializer.Serialize(
                            Static.Settings,
                            new JsonSerializerOptions { WriteIndented = true }
                        )
                    );
                }
                if (OperatingSystem.IsAndroid())
                {
                    var path = Static
                        .StorageProvider?.TryGetWellKnownFolderAsync(WellKnownFolder.Documents)
                        .Result;
                    if (path is not null)
                    {
                        File.WriteAllText(
                            Path.Combine(path.Path.ToString()[8..], "Settings.json"),
                            JsonSerializer.Serialize(
                                Static.Settings,
                                new JsonSerializerOptions { WriteIndented = true }
                            )
                        );
                    }
                }
            }
        }
        public static IStorageProvider? StorageProvider { get; set; }
    }
}
