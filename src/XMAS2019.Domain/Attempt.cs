using System;
using System.Collections.Generic;

namespace XMAS2019.Domain
{
    public class Attempt
    {
        public Attempt(Guid id, string emailAddress, DateTimeOffset created)
        {
            if (string.IsNullOrWhiteSpace(emailAddress)) throw new ArgumentException(@"Value cannot be null or empty", nameof(emailAddress));

            Id = id;
            EmailAddress = emailAddress;
            Created = created;
        }

        public Guid Id { get; }
        public string EmailAddress { get; }
        public DateTimeOffset Created { get; }

        public GeoPoint SantaPosition { get; set; }
        public ReindeerInZone[] ReindeerInZones { get; set; }

        public DateTimeOffset? SantaRescueAt { get; set; }
        public DateTimeOffset? ReindeersRescueAt { get; set; }
        public DateTimeOffset? CompletedAt { get; set; }

        public GeoPoint InvalidSantaPosition { get; set; }
        public string[] InvalidReindeerRescue { get; set; }
        public KeyValuePair<string, string>[] InvalidSolution { get; set; }

        public string Message { get; set; }

        public TimeSpan CalculateExecutionTime()
        {
            if (!CompletedAt.HasValue)
                return TimeSpan.MaxValue;

            return CompletedAt.Value - Created;
        }
    }
}