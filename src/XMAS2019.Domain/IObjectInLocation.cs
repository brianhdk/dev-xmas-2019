namespace XMAS2019.Domain
{
    public interface IObjectInLocation
    {
        string CountryCode { get; }

        string Name { get; }

        GeoPoint GetLocation();
    }
}