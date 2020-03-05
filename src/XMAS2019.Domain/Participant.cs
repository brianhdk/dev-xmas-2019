using System;

namespace XMAS2019.Domain
{
    public class Participant
    {
        public Participant(string fullName, string emailAddress)
        {
            if (string.IsNullOrWhiteSpace(fullName)) throw new ArgumentException(@"Value cannot be null or empty", nameof(fullName));
            if (string.IsNullOrWhiteSpace(emailAddress)) throw new ArgumentException(@"Value cannot be null or empty", nameof(emailAddress));

            FullName = fullName;
            EmailAddress = emailAddress.ToLower();
        }

        public string FullName { get; }
        public string EmailAddress { get; }

        public bool SubscribeToNewsletter { get; set; }

        public override string ToString()
        {
            return $"{FullName} ({EmailAddress})";
        }
    }
}
