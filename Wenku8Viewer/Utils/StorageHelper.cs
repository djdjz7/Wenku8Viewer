using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;

namespace Wenku8Viewer.Utils
{
    internal static class StorageHelper
    {
        public static string? GetSettingsPath()
        {
            if (OperatingSystem.IsWindows())
            {
                return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "Wenku8Viewer",
                    "Settings.json"
                );
            }
            if (OperatingSystem.IsAndroid())
            {
                var path = Static
                    .StorageProvider?.TryGetWellKnownFolderAsync(WellKnownFolder.Documents)
                    .Result;
                if (path is not null)
                    return Path.Combine(path.Path.ToString()[8..], "Settings.json");
            }
            return null;

        }
    }
}
