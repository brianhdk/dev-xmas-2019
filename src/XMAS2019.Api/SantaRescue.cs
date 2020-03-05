using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Vertica.Utilities;
using Vertica.Utilities.Extensions.EnumerableExt;
using XMAS2019.Api.Infrastructure;
using XMAS2019.Api.Messages;
using XMAS2019.Domain;
using XMAS2019.Domain.Services;

namespace XMAS2019.Api
{
    public class SantaRescue
    {
        private readonly IAttemptService _attemptService;
        private readonly IZoneService _zoneService;

        public SantaRescue(IAttemptService attemptService, IZoneService zoneService)
        {
            _attemptService = attemptService ?? throw new ArgumentNullException(nameof(attemptService));
            _zoneService = zoneService ?? throw new ArgumentNullException(nameof(zoneService));
        }

        [FunctionName("SantaRescue")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger logger,
            CancellationToken token)
        {
            return await req.Wrap<SantaRescueRequest>(logger, async request =>
            {
                if (request == null)
                    return new BadRequestErrorMessageResult("Request is empty.");

                if (string.IsNullOrWhiteSpace(request.Id))
                    return new BadRequestErrorMessageResult($"Missing required value for property '{nameof(SantaRescueRequest.Id)}'.");

                if (!Guid.TryParse(request.Id, out Guid id))
                    return new BadRequestErrorMessageResult($"Value for property '{nameof(SantaRescueRequest.Id)}' is not a valid ID or has an invalid format (example of valid value and format for Unique Identifier (GUID): '135996C9-3090-4399-85DB-1F5041DF1DDD').");

                Attempt attempt = await _attemptService.Get(id, 1, token);

                if (attempt == null)
                    return new NotFoundResult();

                if (request.Position == null)
                {
                    attempt.Message = "NoPosition";

                    await _attemptService.Save(attempt, 2, token);

                    return new BadRequestErrorMessageResult($@"Missing required value for property '{nameof(SantaRescueRequest.Position)}'.

Please try again - and remember that since Santa has probably moved his location by now, you'll have to start all over.");
                }

                if (Time.UtcNow - attempt.Created > TimeSpan.FromSeconds(20))
                {
                    attempt.Message = "TookTooLong";

                    await _attemptService.Save(attempt, 2, token);

                    return new BadRequestErrorMessageResult(@"Sorry, Santa has moved his location. It's now been more than 20 seconds since CSAR's tracked the position of Santa's Cane and his movements.

Please try again - and remember that you'll have to start all over.");
                }

                if (!request.Position.Equals(attempt.SantaPosition))
                {
                    SantaTracking santaTracking = await _attemptService.GetSantaTracking(attempt, token);

                    if (!santaTracking.CalculateSantaPositionAlternative().Equals(request.Position))
                    {
                        attempt.InvalidSantaPosition = request.Position;

                        await _attemptService.Save(attempt, 2, token);

                        return new BadRequestErrorMessageResult($@"Unfortunately Santa was not at the specified location. 
Specified location: '{request.Position}', actual location: '{attempt.SantaPosition}'.

Please try again - and remember that since Santa has probably moved his location by now, you'll have to start all over again.

If you continue to get errors, make sure to read the following blog-posts regarding how to implement the algorithm:

 - https://blog.vertica.dk/2019/12/05/hjaelp-til-laage-2-i-vertica-dev-xmas-2019-julekalender/
 - https://blog.vertica.dk/2019/12/04/status-og-information-omkring-laage-2-i-vertica-dev-xmas-2019-julekalender/
");
                    }

                    attempt.Message = "Found Santa with alternative algorithm";
                }

                Reindeer[] reindeers = Enum.GetValues(typeof(Reindeer))
                    .OfType<Reindeer>()
                    .ToArray()
                    .Shuffle()
                    .ToArray();

                IEnumerable<Zone> zones = await _zoneService.GetZonesFor(attempt, reindeers.Length, token);

                attempt.ReindeerInZones = zones
                    .Select((zone, index) => new ReindeerInZone(
                        reindeers[index],
                        zone.CountryCode,
                        zone.CityName,
                        zone.Center,
                        zone.Radius))
                    .ToArray();

                attempt.SantaRescueAt = DateTimeOffset.UtcNow;

                await _attemptService.Save(attempt, 2, token);

                return new OkObjectResult(new SantaRescueResponse(attempt.ReindeerInZones));
            });
        }
    }
}