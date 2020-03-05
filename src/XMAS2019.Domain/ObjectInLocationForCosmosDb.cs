using System;
using Microsoft.Azure.Documents.Spatial;
using Newtonsoft.Json;

namespace XMAS2019.Domain
{
    public class ObjectInLocationForCosmosDb : IObjectInLocation
    {
        public ObjectInLocationForCosmosDb(string id, string countryCode, string name, Point location)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException(@"Value cannot be null or empty", nameof(id));
            if (string.IsNullOrWhiteSpace(countryCode)) throw new ArgumentException(@"Value cannot be null or empty", nameof(countryCode));
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException(@"Value cannot be null or empty", nameof(name));

            Id = id;
            CountryCode = countryCode;
            Name = name;
            Location = location ?? throw new ArgumentNullException(nameof(location));
        }

        [JsonProperty("id")]
        public string Id { get; }

        [JsonProperty("countryCode")]
        public string CountryCode { get; }

        [JsonProperty("name")]
        public string Name { get; }

        [JsonProperty("location")]
        public Point Location { get; }

        public GeoPoint GetLocation() => new GeoPoint(Location.Position.Latitude, Location.Position.Longitude);
    }
}