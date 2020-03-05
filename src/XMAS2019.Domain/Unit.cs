using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace XMAS2019.Domain
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Unit
    {
        /// <summary>
        /// 1 foot => 0.304800610 meters
        /// </summary>
        [EnumMember(Value = "foot")]
        Foot = 0,

        [EnumMember(Value = "meter")]
        Meter = 1,

        [EnumMember(Value = "kilometer")]
        Kilometer = 2
    }
}