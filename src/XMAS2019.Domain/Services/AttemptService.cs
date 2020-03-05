using System;
using System.Threading;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Nest;
using XMAS2019.Domain.Exceptions;

namespace XMAS2019.Domain.Services
{
    public class AttemptService : IAttemptService
    {
        private readonly IElasticClient _elasticClient;

        public AttemptService(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        public async Task<Attempt> Get(Guid id, int expectedVersion, CancellationToken token)
        {
            GetResponse<Attempt> response = await _elasticClient.GetAsync<Attempt>(id, ct: token);

            if (!response.Found)
                return null;

            if (response.Version != expectedVersion)
                throw new AttemptVersionMismatchException(response.Version, expectedVersion);

            return response.Source;
        }

        public async Task Save(Attempt attempt, int expectedVersion, CancellationToken token)
        {
            if (attempt == null) throw new ArgumentNullException(nameof(attempt));

            UpdateResponse<Attempt> response = await _elasticClient
                .UpdateAsync<Attempt, object>(attempt.Id, update => update
                    .Doc(attempt)
                    .Refresh(Refresh.True), token);

            if (response.Version != expectedVersion)
                throw new AttemptVersionMismatchException(response.Version, expectedVersion);
        }

        public async Task<SantaTracking> GetSantaTracking(Attempt attempt, CancellationToken token)
        {
            if (attempt == null) throw new ArgumentNullException(nameof(attempt));

            GetResponse<SantaTracking> response = await _elasticClient.GetAsync<SantaTracking>(attempt.Id, ct: token);

            if (!response.Found)
                throw new InvalidOperationException($"Found no SantaTracking matching Attempt with ID {attempt.Id}.");

            return response.Source;
        }

        public async Task<Statistics> GetStatistics(Attempt lastAttempt, CancellationToken token)
        {
            if (lastAttempt == null) throw new ArgumentNullException(nameof(lastAttempt));

            GetResponse<Participant> response = await _elasticClient.GetAsync<Participant>(lastAttempt.EmailAddress, ct: token);

            if (!response.Found)
                throw new InvalidOperationException($"Found no Participant matching e-mail '{lastAttempt.EmailAddress}'.");

            return new Statistics(response.Source, lastAttempt, response.Version);
        }
    }
}