using System;
using NGeoHash;

namespace XMAS2019.Domain
{
    public class GeoPoint : IEquatable<GeoPoint>
    {
        public GeoPoint(double lat, double lon)
        {
            Lat = lat;
            Lon = lon;
        }

        public double Lat { get; }
        public double Lon { get; }

        public override string ToString()
        {
            return $"{Lat}, {Lon}";
        }

        public bool Equals(GeoPoint other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            // https://en.wikipedia.org/wiki/Decimal_degrees
            // Decimal places = 6 (individual humans)

            return
                Math.Abs(Lat - other.Lat) < 0.0001 &&
                Math.Abs(Lon - other.Lon) < 0.0001;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((GeoPoint)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Lat.GetHashCode() * 397) ^ Lon.GetHashCode();
            }
        }

        /// <summary>
        /// https://stackoverflow.com/questions/7477003/calculating-new-longitude-latitude-from-old-n-meters
        /// </summary>
        public GeoPoint MoveTo(double xMeters, double yMeters)
        {
            return new GeoPoint(
                MoveLatitude(Lat, yMeters),
                MoveLongitude(Lon, Lat, xMeters));

            double MoveLatitude(double latitude, double meters)
            {
                if (meters.Equals(0d))
                    return latitude;

                return latitude + meters * M;
            }

            double MoveLongitude(double longitude, double latitude, double meters)
            {
                if (meters.Equals(0d))
                    return longitude;

                return longitude + meters * M / Math.Cos(latitude * (Math.PI / 180));
            }
        }

        public string GetGeoHash() => GeoHash.Encode(Lat, Lon, 11);

        /// <summary>
        /// 1 meter in degree (with radius of the earth in kilometer)
        /// </summary>
        private const double M = 1 / (2 * Math.PI / 360 * 6378.137) / 1000;
    }
}