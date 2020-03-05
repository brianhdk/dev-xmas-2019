using XMAS2019.Domain;

namespace XMAS2019.Api.Messages
{
    public class ReindeerRescueRequest
    {
        public string Id { get; set; }

        public ReindeerLocation[] Locations { get; set; }

        public class ReindeerLocation
        {
            public string Name { get; set; }

            public GeoPoint Position { get; set; }
        }
    }
}