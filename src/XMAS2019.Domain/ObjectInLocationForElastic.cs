using System;

namespace XMAS2019.Domain
{
    public class ObjectInLocationForElastic : IObjectInLocation
    {
        public ObjectInLocationForElastic(string countryCode, string name, GeoPoint location)
        {
            if (string.IsNullOrWhiteSpace(countryCode)) throw new ArgumentException(@"Value cannot be null or empty", nameof(countryCode));
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException(@"Value cannot be null or empty", nameof(name));

            CountryCode = countryCode;
            Name = name;
            Location = location ?? throw new ArgumentNullException(nameof(location));
        }

        public string CountryCode { get; }
        public string Name { get; }
        public GeoPoint Location { get; }

        public GeoPoint GetLocation() => Location;
    }
}