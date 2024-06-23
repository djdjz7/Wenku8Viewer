using System.Text.Json;

namespace Wenku8Viewer.Utils
{
    public static class Debugging
    {
        public static string DebugDump(this object obj)
        {
            return JsonSerializer.Serialize(
                obj,
                new JsonSerializerOptions { WriteIndented = true }
            );
        }
    }
}
