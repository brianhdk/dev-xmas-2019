using System;

namespace XMAS2019.Api.Messages
{
    public class ParticipateResponse
    {
        public ParticipateResponse(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }

        public ElasticCredentials Credentials => new ElasticCredentials("Participant", "fr5ZS6NT2gQE1VL0hLZmB1X8HhGAW4");

        public class ElasticCredentials
        {
            public ElasticCredentials(string username, string password)
            {
                Username = username;
                Password = password;
            }

            public string Username { get; }
            public string Password { get; }
        }
    }
}