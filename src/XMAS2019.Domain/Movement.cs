using Elasticsearch.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace XMAS2019.Domain
{
    public class Movement
    {
        public Movement(Direction direction, double value, Unit unit)
        {
            Direction = direction;
            Value = value;
            Unit = unit;
        }

        [StringEnum]
        [JsonConverter(typeof(StringEnumConverter))]
        public Direction Direction { get; }

        public double Value { get; }

        [StringEnum]
        [JsonConverter(typeof(StringEnumConverter))]
        public Unit Unit { get; }
    }
}