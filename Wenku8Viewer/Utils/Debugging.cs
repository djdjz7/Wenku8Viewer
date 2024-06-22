using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
