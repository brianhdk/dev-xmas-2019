using Elasticsearch.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace XMAS2019.Domain
{
    public class Radius
    {
        public Radius(Unit unit, double value)
        {
            Unit = unit;
            Value = value;
        }

        [JsonConverter(typeof(StringEnumConverter))]
        [StringEnum]
        public Unit Unit { get; }

        public double Value { get; }

        public double InMeter()
        {
            return Unit.ToMeters(Value);
        }

        public override string ToString()
        {
            return $"{Value} {Unit}";
        }
    }
}