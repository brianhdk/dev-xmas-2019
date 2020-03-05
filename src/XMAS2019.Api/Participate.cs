using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Elasticsearch.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Nest;
using XMAS2019.Api.Infrastructure;
using XMAS2019.Api.Messages;
using XMAS2019.Domain;

namespace XMAS2019.Api
{
    public class Participate
    {
        private static readonly Random Random = new Random(Guid.NewGuid().GetHashCode());
        private static readonly GeoPoint GreenlandCenter = new GeoPoint(71.7069397, -42.6043015);

        private readonly IElasticClient _elasticClient;

        public Participate(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient ?? throw new ArgumentNullException(nameof(elasticClient));
        }

        [FunctionName("Participate")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger logger,
            CancellationToken token)
        {
            return await req.Wrap<ParticipateRequest>(logger, async request =>
            {
                if (request == null)
                    return new BadRequestErrorMessageResult("Request is empty.");

                if (string.IsNullOrWhiteSpace(request.FullName))
                    return new BadRequestErrorMessageResult($"Missing required value for property '{nameof(ParticipateRequest.FullName)}'.");

                if (request.FullName.Length > 100)
                    return new BadRequestErrorMessageResult($"Property '{nameof(ParticipateRequest.FullName)}' cannot have more than 100 characters.");

                if (string.IsNullOrWhiteSpace(request.EmailAddress))
                    return new BadRequestErrorMessageResult($"Missing required value for property '{nameof(ParticipateRequest.EmailAddress)}'.");

                if (request.EmailAddress.Length > 100)
                    return new BadRequestErrorMessageResult($"Property '{nameof(ParticipateRequest.EmailAddress)}' cannot have more than 100 characters.");

                if (!Regex.IsMatch(request.EmailAddress, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$"))
                    return new BadRequestErrorMessageResult($"Value for property '{nameof(ParticipateRequest.EmailAddress)}' is not a valid e-mail address.");

                var participant = new Participant(request.FullName, request.EmailAddress)
                {
                    SubscribeToNewsletter = request.SubscribeToNewsletter.GetValueOrDefault()
                };

                // In 50K radius of Greenland center
                GeoPoint canePosition = GreenlandCenter.MoveTo(
                    Random.Next(-50000, 50001),
                    Random.Next(-50000, 500001));

                IEnumerable<Movement> movements = GenerateMovements(Random.Next(150, 201));

                var attempt = new Attempt(Guid.NewGuid(), participant.EmailAddress, DateTimeOffset.UtcNow);

                var santaTracking = new SantaTracking(attempt.Id, canePosition, movements.ToArray());

                attempt.SantaPosition = santaTracking.CalculateSantaPosition();

                await _elasticClient.BulkAsync(selector => selector
                    .Index<Participant>(s => s.Document(participant))
                    .Index<Attempt>(s => s.Document(attempt))
                    .Index<SantaTracking>(s => s.Document(santaTracking))
                    .Refresh(Refresh.True), token);

                return new OkObjectResult(new ParticipateResponse(attempt.Id));
            });
        }

        private static IEnumerable<Movement> GenerateMovements(int count)
        {
            return Enumerable.Range(1, count).Select(x =>
            {
                var direction = (Direction)Random.Next(0, 4);
                var unit = (Unit)Random.Next(0, 3);
                double value;

                switch (unit)
                {
                    case Unit.Foot:
                        value = Random.Next(1, 800);
                        break;

                    case Unit.Meter:
                    case Unit.Kilometer:

                        double meters = Random.Next(1, 251);
                        value = meters.FromMeters(unit);

                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return new Movement(direction, value, unit);
            });
        }
    }
}