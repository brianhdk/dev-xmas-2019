namespace XMAS2019.Api.Messages
{
    public class ParticipateRequest
    {
        public string FullName { get; set; }
        public string EmailAddress { get; set; }
        public bool? SubscribeToNewsletter { get; set; }
    }
}