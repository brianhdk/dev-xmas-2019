using System;

namespace XMAS2019.Api.Messages
{
    public class ReindeerRescueResponse
    {
        public ReindeerRescueResponse(Uri toyDistributionXmlUrl)
        {
            ToyDistributionXmlUrl = toyDistributionXmlUrl ?? throw new ArgumentNullException(nameof(toyDistributionXmlUrl));
        }

        public Uri ToyDistributionXmlUrl { get; }

        public string Message => "Good job! You've successfully found all Santa's reindeer. Information about the next and final challenge (door 4) is available on https://blog.vertica.dk/category/dev-xmas-2019-julekalender/.";
    }
}