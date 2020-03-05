using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Nest;

namespace XMAS2019.Domain.Infrastructure
{
    public class ElasticInitializer : IElasticInitializer
    {
        private readonly IElasticClient _elasticClient;

        public ElasticInitializer(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient ?? throw new ArgumentNullException(nameof(elasticClient));
        }

        public async Task Initialize(CancellationToken token)
        {
            await EnsureIndex<Participant>("participants", map => map
                .AutoMap()
                .Properties(properties => properties
                    .Keyword(property => property.Name(x => x.FullName).Index(false))
                ), token);

            await EnsureIndex<Attempt>("attempts", map => map
                .Properties(properties => properties
                    .GeoPoint(property => property.Name(x => x.SantaPosition))
                    .GeoPoint(property => property.Name(x => x.InvalidSantaPosition))
                    .Keyword(property => property.Name(x => x.EmailAddress).Index(false))
                    .Keyword(property => property.Name(PropertyPathName(nameof(Attempt.ReindeerInZones), nameof(ReindeerInZone.CountryCode))).Index(false))
                    .Keyword(property => property.Name(PropertyPathName(nameof(Attempt.ReindeerInZones), nameof(ReindeerInZone.CityName))).Index(false))
                    .GeoPoint(property => property.Name(PropertyPathName(nameof(Attempt.ReindeerInZones), nameof(ReindeerInZone.Center))))
                    .Keyword(property => property.Name(x => x.Message).Index(false))

            ), token);

            await EnsureIndex<SantaTracking>("santa-trackings", map => map
                .AutoMap()
                .Properties(properties => properties
                    .GeoPoint(property => property.Name(x => x.CanePosition))
                ), token);

            await EnsureIndex<Zone>("zones", map => map
                .AutoMap()
                .Properties(properties => properties
                    .Keyword(property => property.Name(x => x.CountryCode).Index(false))
                    .Keyword(property => property.Name(x => x.CityName).Index(false))
                    .GeoPoint(property => property.Name(x => x.Center))
                ), token);

            await EnsureIndex<ObjectInLocationForElastic>("objects", map => map
                .AutoMap()
                .Properties(properties => properties
                    .Keyword(property => property.Name(x => x.CountryCode).Index(false))
                    .Keyword(property => property.Name(x => x.Name).Index(false))
                    .GeoPoint(property => property.Name(x => x.Location))
                ), token);
        }

        private static string PropertyPathName(params string[] paths)
        {
            return string.Join(".", paths.Select(x => x.WithFirstCharToLowercase()));
        }

        private async Task EnsureIndex<T>(string name, Func<TypeMappingDescriptor<T>, ITypeMapping> mapper, CancellationToken token) where T : class
        {
            if ((await _elasticClient.Indices.ExistsAsync(name, ct: token)).Exists)
                await _elasticClient.Indices.DeleteAsync(name, ct: token);

            var response = await _elasticClient.Indices.CreateAsync(name, create => create.Map(mapper), token);

            if (!response.IsValid)
                throw response.OriginalException;
        }
    }
}