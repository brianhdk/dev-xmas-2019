using System;

namespace XMAS2019.Domain
{
    public static class Lengths
    {
        public static double MetersToKilometers(this double meters)
        {
            return meters / 1000;
        }

        public static double KilometersToMeters(this double kilometers)
        {
            return kilometers * 1000;
        }

        public static double MetersToFeet(this double meters)
        {
            return meters / 0.304800610;
        }

        public static double FeetToMeters(this double feet)
        {
            return feet * 0.304800610;
        }

        public static double ToMeters(this Unit unit, double value)
        {
            switch (unit)
            {
                case Unit.Foot:
                    return value.FeetToMeters();

                case Unit.Meter:
                    return value;

                case Unit.Kilometer:
                    return value.KilometersToMeters();

                default:
                    throw new ArgumentOutOfRangeException(nameof(unit), unit, null);
            }
        }

        public static double FromMeters(this double meters, Unit toUnit)
        {
            switch (toUnit)
            {
                case Unit.Foot:
                    return meters.MetersToFeet();
                case Unit.Meter:
                    return meters;
                case Unit.Kilometer:
                    return meters.MetersToKilometers();
                default:
                    throw new ArgumentOutOfRangeException(nameof(toUnit), toUnit, null);
            }
        }
    }
}