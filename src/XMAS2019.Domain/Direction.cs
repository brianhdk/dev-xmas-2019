using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace XMAS2019.Domain
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Direction
    {
        [EnumMember(Value = "up")]
        Up = 0,

        [EnumMember(Value = "right")]
        Right = 1,

        [EnumMember(Value = "down")]
        Down = 2,

        [EnumMember(Value = "left")]
        Left = 3
    }
}