namespace XMAS2019.Api.Messages
{
    public class ToyDistributionRequest
    {
        public string Id { get; set; }

        public ChildGettingToy[] ToyDistribution { get; set; }

        public class ChildGettingToy
        {
            public string ChildName { get; set; }
            public string ToyName { get; set; }
        }
    }
}