using System.Text.Json.Serialization;

namespace mGBAHttp.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum KeysEnum
    {
        A,
        B,
        Select,
        Start,
        Right,
        Left,
        Up,
        Down,
        R,
        L
    }
}
