using System;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using XMAS2019.Domain.Exceptions;
using XMAS2019.Domain.Infrastructure;

namespace XMAS2019.Api.Infrastructure
{
    public static class RequestExtensions
    {
        private static readonly DateTimeOffset CompetitionEnds = new DateTimeOffset(2019, 12, 20, 12, 00, 00, TimeSpan.FromHours(1));

        public static async Task<IActionResult> Wrap<T>(this HttpRequest httpRequest, ILogger logger, Func<T, Task<IActionResult>> action)
            where T : class
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (action == null) throw new ArgumentNullException(nameof(action));

            if (DateTimeOffset.UtcNow >= CompetitionEnds)
            {
                logger.LogWarning("TooLate");

                return new BadRequestErrorMessageResult("Sorry, you're too late. The Christmas competition is over. See all winners and prizes on https://blog.vertica.dk/category/dev-xmas-2019-julekalender/. Merry Christmas!'");
            }

            string body = await httpRequest.Read(logger);

            T request;

            try
            {
                request = JsonConvert.DeserializeObject<T>(body);
            }
            catch (JsonException ex)
            {
                logger.LogError(ex, "BadRequest with message: Error parsing body ({Message}) {Body}", ex.Message, body.MaxLengthWithDots(3000));

                return new BadRequestErrorMessageResult($"Request is invalid. Error parsing body as JSON: {ex.Message}.");
            }

            try
            {
                IActionResult result = await action(request);

                if (result is BadRequestErrorMessageResult badRequest)
                {
                    logger.LogWarning("BadRequest with message: {Message}, {@Request}", badRequest.Message, request);
                }
                else if (result is NotFoundResult)
                {
                    logger.LogWarning("NotFound {@Request}", request);
                }

                return result;
            }
            catch (AttemptVersionMismatchException ex)
            {
                logger.LogWarning(ex, "AttemptVersionMismatch: {Message}, {@Request}", ex.Message, request);

                return new BadRequestErrorMessageResult("Unfortunately you cannot reuse the same ID - you'll have to start all over by calling the 'api/participate' endpoint, to receive a new ID.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled exception {Message}, {@Request}", ex.Message, request);

                throw;
            }
        }

        private static async Task<string> Read(this HttpRequest request, ILogger logger)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            using var reader = new StreamReader(request.Body);

            try
            {
                return await reader.ReadToEndAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error reading body");

                return default;
            }
        }
    }
}