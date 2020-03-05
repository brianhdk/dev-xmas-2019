using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Nest;

namespace XMAS2019.Domain.Services
{
    public class ZoneService : IZoneService
    {
        private readonly IElasticClient _elasticClient;

        public ZoneService(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        public async Task<IEnumerable<Zone>> GetZonesFor(Attempt attempt, int count, CancellationToken token)
        {
            if (attempt == null) throw new ArgumentNullException(nameof(attempt));

            // https://stackoverflow.com/questions/25887850/random-document-in-elasticsearch

            ISearchResponse<Zone> response = await _elasticClient.SearchAsync<Zone>(search => search
                .Size(8)
                .Query(query => query
                    .FunctionScore(score => score
                        .Functions(functions => functions
                            .RandomScore(randomScore => randomScore
                                .Seed(attempt.Id.GetHashCode())
                            )
                        )
                    )
                ), token);

            return response.Documents;
        }
        
    }
}