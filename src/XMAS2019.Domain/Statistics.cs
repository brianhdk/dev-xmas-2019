using System;

namespace XMAS2019.Domain
{
    public class Statistics
    {
        public Statistics(Participant participant, Attempt lastAttempt, long numberOfAttempts)
        {
            Participant = participant ?? throw new ArgumentNullException(nameof(participant));
            LastAttempt = lastAttempt ?? throw new ArgumentNullException(nameof(lastAttempt));
            NumberOfAttempts = numberOfAttempts;
        }

        public Participant Participant { get; }
        public Attempt LastAttempt { get; }
        public long NumberOfAttempts { get; }
    }
}