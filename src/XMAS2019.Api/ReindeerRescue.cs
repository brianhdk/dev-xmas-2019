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
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Spatial;
using Microsoft.Extensions.Logging;
using XMAS2019.Api.Infrastructure;
using XMAS2019.Api.Messages;
using XMAS2019.Domain;
using XMAS2019.Domain.Infrastructure;
using XMAS2019.Domain.Services;

namespace XMAS2019.Api
{
    public class ReindeerRescue
    {
        private readonly IAttemptService _attemptService;
        private readonly IDocumentClient _documentClient;
        private readonly IToyDistributionProblemRepository _toyDistributionProblemRepository;

        public ReindeerRescue(IAttemptService attemptService, IDocumentClient documentClient, IToyDistributionProblemRepository toyDistributionProblemRepository)
        {
            _attemptService = attemptService ?? throw new ArgumentNullException(nameof(attemptService));
            _documentClient = documentClient ?? throw new ArgumentNullException(nameof(documentClient));
            _toyDistributionProblemRepository = toyDistributionProblemRepository ?? throw new ArgumentNullException(nameof(toyDistributionProblemRepository));
        }

        [FunctionName("ReindeerRescue")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger logger,
            CancellationToken token)
        {
            return await req.Wrap<ReindeerRescueRequest>(logger, async request =>
            {
                if (request == null)
                    return new BadRequestErrorMessageResult("Request is empty.");

                if (string.IsNullOrWhiteSpace(request.Id))
                    return new BadRequestErrorMessageResult($"Missing required value for property '{nameof(ReindeerRescueRequest.Id)}'.");

                if (!Guid.TryParse(request.Id, out Guid id))
                    return new BadRequestErrorMessageResult($"Value for property '{nameof(ReindeerRescueRequest.Id)}' is not a valid ID or has an invalid format (example of valid value and format for Unique Identifier (GUID): '135996C9-3090-4399-85DB-1F5041DF1DDD').");

                Attempt attempt = await _attemptService.Get(id, 2, token);

                if (attempt == null)
                    return new NotFoundResult();

                if (!attempt.SantaRescueAt.HasValue)
                    return new BadRequestErrorMessageResult($"It looks like you're reusing an old ID from a previous failed attempt created {attempt.Created} - you need to start all over again - remember not to hard code any IDs in your application.");

                if (attempt.ReindeerInZones == null)
                    throw new InvalidOperationException($"Missing value for '{nameof(attempt.ReindeerInZones)}' on attempt with Id '{attempt.Id}'.");

                var messages = new List<string>();

                if (request.Locations == null)
                {
                    messages.Add($"Missing required value for property '{nameof(ReindeerRescueRequest.Locations).WithFirstCharToLowercase()}'.");
                }
                else if (request.Locations.Length > attempt.ReindeerInZones.Length)
                {
                    messages.Add($"Invalid value for property '{nameof(ReindeerRescueRequest.Locations).WithFirstCharToLowercase()}'. Expected a maximum number of locations to be {attempt.ReindeerInZones.Length} - but was {request.Locations.Length}.");
                }
                else
                {
                    for (int i = 0; i < attempt.ReindeerInZones.Length; i++)
                    {
                        ReindeerInZone zone = attempt.ReindeerInZones[i];
                        ReindeerRescueRequest.ReindeerLocation locationInRequest = request.Locations?.ElementAtOrDefault(i);

                        if (locationInRequest == null)
                        {
                            messages.Add($"Missing required value for property '{nameof(ReindeerRescueRequest.Locations).WithFirstCharToLowercase()}[{i}]'.");
                        }
                        else if (string.IsNullOrWhiteSpace(locationInRequest.Name))
                        {
                            messages.Add($"Missing required value for property '{nameof(ReindeerRescueRequest.Locations).WithFirstCharToLowercase()}[{i}].{nameof(ReindeerRescueRequest.ReindeerLocation.Name).WithFirstCharToLowercase()}'");
                        }
                        else if (locationInRequest.Name.Length >= 100)
                        {
                            messages.Add($"Invalid value for property '{nameof(ReindeerRescueRequest.Locations).WithFirstCharToLowercase()}[{i}].{nameof(ReindeerRescueRequest.ReindeerLocation.Name).WithFirstCharToLowercase()}' - length exceeds maximum of 100 characters.");
                        }
                        else if (!string.Equals(locationInRequest.Name, zone.Reindeer.ToString()))
                        {
                            messages.Add($"Expected name of Reindeer for '{nameof(ReindeerRescueRequest.Locations).WithFirstCharToLowercase()}[{i}].{nameof(ReindeerRescueRequest.ReindeerLocation.Name).WithFirstCharToLowercase()}' to be '{zone.Reindeer}' - but was '{locationInRequest.Name}'.");
                        }
                        else if (locationInRequest.Position == null)
                        {
                            messages.Add($"Missing required value for property '{nameof(ReindeerRescueRequest.Locations).WithFirstCharToLowercase()}[{i}].{nameof(ReindeerRescueRequest.ReindeerLocation.Position).WithFirstCharToLowercase()}'");
                        }
                        else
                        {
                            Point center = new Point(zone.Center.Lon, zone.Center.Lat);
                            double radiusInMeter = zone.Radius.InMeter();
                            string name = zone.Reindeer.ToString();

                            var feedOptions = new FeedOptions
                            {
                                PartitionKey = new PartitionKey(zone.CountryCode),
                                MaxItemCount = 1
                            };

                            ObjectInLocationForCosmosDb actualReindeer = _documentClient
                                .CreateDocumentQuery<ObjectInLocationForCosmosDb>(UriFactory.CreateDocumentCollectionUri("World", "Objects"), feedOptions)
                                .Where(u => u.Name == name && center.Distance(u.Location) <= radiusInMeter)
                                .AsEnumerable()
                                .FirstOrDefault();

                            if (actualReindeer == null)
                                throw new InvalidOperationException($"Reindeer '{name}' not found for zone {zone.Center}, Country {zone.CountryCode}.");

                            GeoPoint actualPosition = actualReindeer.GetLocation();

                            if (!actualPosition.Equals(locationInRequest.Position))
                            {
                                messages.Add($"Invalid value for property '{nameof(ReindeerRescueRequest.Locations).WithFirstCharToLowercase()}[{i}].{nameof(ReindeerRescueRequest.ReindeerLocation.Position).WithFirstCharToLowercase()}'. Reindeer '{zone.Reindeer}' was not found at {locationInRequest.Position} - actual position is {actualPosition}.");
                            }
                        }
                    }
                }

                if (messages.Count > 0)
                {
                    attempt.InvalidReindeerRescue = messages.ToArray();

                    await _attemptService.Save(attempt, 3, token);

                    return new BadRequestErrorMessageResult($@"Unfortunately one or more reindeer was not at the specified locations. 
Below is the detailed rescue report from the extraction team:

{string.Join(Environment.NewLine, messages.Select(x => $"\t - {x}"))}

Please try again - and remember since the reindeer has probably moved their locations by now, you'll have to start all over again.");
                }

                attempt.ReindeersRescueAt = DateTimeOffset.UtcNow;

                Uri problemUri = await _toyDistributionProblemRepository.Save(attempt, ToyDistributionProblem.Create(), token);

                await _attemptService.Save(attempt, 3, token);

                return new OkObjectResult(new ReindeerRescueResponse(problemUri));
            });
        }
    }
}