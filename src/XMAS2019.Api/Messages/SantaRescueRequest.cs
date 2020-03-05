using XMAS2019.Domain;

namespace XMAS2019.Api.Messages
{
    public class SantaRescueRequest
    {
        public string Id { get; set; }

        public GeoPoint Position { get; set; }
    }
}