using System.Text.Json.Serialization;

namespace mGBAHttp.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ButtonEnum
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
