using System;
using System.Collections.Generic;
using XMAS2019.Domain;
using XMAS2019.Domain.Infrastructure;

namespace XMAS2019.Api.Messages
{
    public class SantaRescueResponse
    {
        public SantaRescueResponse(IEnumerable<ReindeerInZone> zones)
        {
            Zones = zones ?? throw new ArgumentNullException(nameof(zones));
        }

        public IEnumerable<ReindeerInZone> Zones { get; }

        public string Token => DocumentClientFactory.ReadOnlyAuthKey;
    }
}