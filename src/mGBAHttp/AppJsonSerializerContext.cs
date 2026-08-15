using System.Text.Json.Serialization;
using mGBAHttp.Models;

namespace mGBAHttp
{
    // Source generated JSON so serialization works when trimming.
    // Used by Scalar for parameter examples
    [JsonSerializable(typeof(ButtonEnum))]
    [JsonSerializable(typeof(ButtonEnum[]))]
    [JsonSerializable(typeof(string))]
    [JsonSerializable(typeof(int))]
    [JsonSerializable(typeof(bool))]
    [JsonSerializable(typeof(uint))]
    internal partial class AppJsonSerializerContext : JsonSerializerContext { }
}
