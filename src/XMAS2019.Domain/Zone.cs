using System;

namespace XMAS2019.Domain
{
    public class Zone : IEquatable<Zone>
    {
        public Zone(string countryCode, string cityName, GeoPoint center, Radius radius)
        {
            if (string.IsNullOrWhiteSpace(countryCode)) throw new ArgumentException(@"Value cannot be null or empty", nameof(countryCode));
            if (string.IsNullOrWhiteSpace(cityName)) throw new ArgumentException(@"Value cannot be null or empty", nameof(cityName));

            CountryCode = countryCode;
            CityName = cityName;
            
            Center = center ?? throw new ArgumentNullException(nameof(center));
            Radius = radius ?? throw new ArgumentNullException(nameof(radius));
        }

        public string CountryCode { get; }
        public string CityName { get; }

        public GeoPoint Center { get; }

        public Radius Radius { get; }

        public override string ToString()
        {
            return $"{CountryCode}, {CityName}, {Center} with radius {Radius}";
        }

        public bool Equals(Zone other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Center.Equals(other.Center);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Zone) obj);
        }

        public override int GetHashCode()
        {
            return Center.GetHashCode();
        }
    }
}