using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using XMAS2019.Api.Infrastructure;
using XMAS2019.Api.Messages;
using XMAS2019.Domain;
using XMAS2019.Domain.Infrastructure;
using XMAS2019.Domain.Services;

namespace XMAS2019.Api
{
    public class ToyDistribution
    {
        private readonly IAttemptService _attemptService;
        private readonly IToyDistributionProblemRepository _toyDistributionProblemRepository;

        public ToyDistribution(IAttemptService attemptService, IToyDistributionProblemRepository toyDistributionProblemRepository)
        {
            _attemptService = attemptService ?? throw new ArgumentNullException(nameof(attemptService));
            _toyDistributionProblemRepository = toyDistributionProblemRepository ?? throw new ArgumentNullException(nameof(toyDistributionProblemRepository));
        }

        [FunctionName("ToyDistribution")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger logger,
            CancellationToken token)
        {
            return await req.Wrap<ToyDistributionRequest>(logger, async request =>
            {
                if (request == null)
                    return new BadRequestErrorMessageResult("Request is empty.");

                if (string.IsNullOrWhiteSpace(request.Id))
                    return new BadRequestErrorMessageResult($"Missing required value for property '{nameof(SantaRescueRequest.Id)}'.");

                if (!Guid.TryParse(request.Id, out Guid id))
                    return new BadRequestErrorMessageResult($"Value for property '{nameof(SantaRescueRequest.Id)}' is not a valid ID or has an invalid format (example of valid value and format for Unique Identifier (GUID): '135996C9-3090-4399-85DB-1F5041DF1DDD').");

                Attempt attempt = await _attemptService.Get(id, 3, token);

                if (attempt == null)
                    return new NotFoundResult();

                if (!attempt.ReindeersRescueAt.HasValue)
                    return new BadRequestErrorMessageResult($"It looks like you're reusing an old ID from a previous failed attempt created {attempt.Created} - you need to start all over again - remember not to hard code any IDs in your application.");

                var messages = new List<string>();

                if (request.ToyDistribution == null)
                {
                    messages.Add($"Missing required value for property '{nameof(ToyDistributionRequest.ToyDistribution).WithFirstCharToLowercase()}'.");
                }
                else if (request.ToyDistribution.Length != ToyDistributionProblem.ListLength)
                {
                    messages.Add($"Invalid value for property '{nameof(ToyDistributionRequest.ToyDistribution).WithFirstCharToLowercase()}'. Expected number of elements to be {ToyDistributionProblem.ListLength} - but was {request.ToyDistribution.Length}.");
                }
                else
                {
                    for (int i = 0; i < request.ToyDistribution.Length; i++)
                    {
                        ToyDistributionRequest.ChildGettingToy pair = request.ToyDistribution[i];

                        if (pair == null)
                        {
                            messages.Add($"Missing required value for property '{nameof(ToyDistributionRequest.ToyDistribution).WithFirstCharToLowercase()}[{i}]'.");
                        }
                        else if (string.IsNullOrWhiteSpace(pair.ChildName))
                        {
                            messages.Add($"Missing required value for property '{nameof(ToyDistributionRequest.ToyDistribution).WithFirstCharToLowercase()}[{i}].{nameof(ToyDistributionRequest.ChildGettingToy.ChildName).WithFirstCharToLowercase()}'");
                        }
                        else if (pair.ChildName.Length >= 100)
                        {
                            messages.Add($"Invalid value for property '{nameof(ToyDistributionRequest.ToyDistribution).WithFirstCharToLowercase()}[{i}].{nameof(ToyDistributionRequest.ChildGettingToy.ChildName).WithFirstCharToLowercase()}' - length exceeds maximum of 100 characters.");
                        }
                        else if (string.IsNullOrWhiteSpace(pair.ToyName))
                        {
                            messages.Add($"Missing required value for property '{nameof(ToyDistributionRequest.ToyDistribution).WithFirstCharToLowercase()}[{i}].{nameof(ToyDistributionRequest.ChildGettingToy.ToyName).WithFirstCharToLowercase()}'");
                        }
                        else if (pair.ToyName.Length >= 100)
                        {
                            messages.Add($"Invalid value for property '{nameof(ToyDistributionRequest.ToyDistribution).WithFirstCharToLowercase()}[{i}].{nameof(ToyDistributionRequest.ChildGettingToy.ToyName).WithFirstCharToLowercase()}' - length exceeds maximum of 100 characters.");
                        }
                    }

                    string[] duplicateChildNames = request.ToyDistribution
                        .Where(x => !string.IsNullOrWhiteSpace(x?.ChildName))
                        .GroupBy(x => x.ChildName, StringComparer.OrdinalIgnoreCase)
                        .Where(x => x.Count() > 1)
                        .Select(x => x.Key)
                        .ToArray();

                    if (duplicateChildNames.Length > 0)
                    {
                        messages.Add($"You have the same child name mentioned more than once in the request. This is not allowed. Duplicate name(s): {string.Join(", ", duplicateChildNames)}");
                    }

                    string[] duplicateToyNames = request.ToyDistribution
                        .Where(x => !string.IsNullOrWhiteSpace(x?.ToyName))
                        .GroupBy(x => x.ToyName, StringComparer.OrdinalIgnoreCase)
                        .Where(x => x.Count() > 1)
                        .Select(x => x.Key)
                        .ToArray();

                    if (duplicateToyNames.Length > 0)
                    {
                        messages.Add($"You have the same toy name mentioned more than once in the request. This is not allowed. Duplicate name(s): {string.Join(", ", duplicateToyNames)}");
                    }
                }

                if (messages.Count == 0)
                {
                    ToyDistributionProblem problem = await _toyDistributionProblemRepository.Get(attempt, token);

                    if (problem == null)
                        throw new InvalidOperationException($"No problem found for {attempt.Id}.");

                    string[] missingChildren = problem.Children
                        .Select(x => x.Name)
                        .Except(request.ToyDistribution?.Select(x => x.ChildName) ?? new string[0])
                        .ToArray();

                    if (missingChildren.Length > 0)
                    {
                        messages.Add($@"One or more children names are missing from your list: 
 - Expected: {string.Join(", ", problem.Children.Select(x => x.Name))}
 - Actual: {string.Join(", ", request.ToyDistribution?.Select(x => x.ChildName) ?? new string[0])}");
                    }
                    else
                    {
                        var proposedSolution = new ToyDistributionSolution
                        {
                            List = request.ToyDistribution?.ToDictionary(x => x.ChildName, x => x.ToyName)
                        };

                        if (!proposedSolution.IsValidFor(problem))
                        {
                            ToyDistributionSolution validSolution = problem.CreateSolution();

                            messages.Add($@"The proposed solution was not found correct. Below is a correct solution to the problem:
{string.Join(Environment.NewLine, validSolution.List.Select(x => $"\t\t- '{x.Key}' gets '{x.Value}'"))}");
                        }
                    }
                }

                if (messages.Count > 0)
                {
                    attempt.InvalidSolution = request.ToyDistribution?
                        .Take(ToyDistributionProblem.ListLength * 2)
                        .Select(x => new KeyValuePair<string, string>(
                            x?.ChildName.MaxLengthWithDots(100), 
                            x?.ToyName.MaxLengthWithDots(100)))
                        .ToArray();

                    await _attemptService.Save(attempt, 4, token);

                    return new BadRequestErrorMessageResult($@"Unfortunately you have one or more errors:

{string.Join(Environment.NewLine, messages.Select(x => $"\t - {x}"))}

Please try again - and remember, as always, you'll have to start all over again.");
                }

                attempt.CompletedAt = DateTimeOffset.UtcNow;

                await _attemptService.Save(attempt, 4, token);

                return new OkObjectResult(new ToyDistributionResponse(await _attemptService.GetStatistics(attempt, token)));
            });
        }
    }
}
