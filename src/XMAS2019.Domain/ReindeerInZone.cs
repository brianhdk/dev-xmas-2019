using System;
using System.ComponentModel;
using Elasticsearch.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace XMAS2019.Domain
{
    public class ReindeerInZone : Zone
    {
        public ReindeerInZone(Reindeer reindeer, string countryCode, string cityName, GeoPoint center, Radius radius)
            : base(countryCode, cityName, center, radius)
        {
            if (!Enum.IsDefined(typeof(Reindeer), reindeer)) throw new InvalidEnumArgumentException(nameof(reindeer), (int) reindeer, typeof(Reindeer));

            Reindeer = reindeer;
        }

        [StringEnum]
        [JsonConverter(typeof(StringEnumConverter))]
        public Reindeer Reindeer { get; }
    }
}