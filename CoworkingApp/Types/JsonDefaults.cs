using System.Text.Json;
using System.Text.Json.Serialization;

namespace CoworkingApp.Types
{
    public static class JsonDefaults
    {
        public static JsonSerializerOptions NO_CYCLES => new()
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            //WriteIndented = true
        };

    }
}
