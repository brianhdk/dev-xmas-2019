using System;
using System.Text;
using Elasticsearch.Net;
using Nest;

namespace XMAS2019.Domain.Infrastructure
{
    public static class ElasticClientFactory
    {
        // NOTE: Never hard-code resource URIs - use configuration
        public static Uri Url => new Uri("https://eebc6f27713d4519970d75c86050e363.eu-central-1.aws.cloud.es.io");

        public static string Username => "..."; // NOTE: Never hard-code credentials - use Azure Key Vault
        public static string Password => "..."; // NOTE: Never hard-code credentials - use Azure Key Vault

        public static IElasticClient Create(Action<string> debugger = null)
        {
            // NOTE: Never hard-code credentials - Use Azure Key Vault
            var settings = new ConnectionSettings("...", new BasicAuthenticationCredentials(Username, Password))
                .DefaultMappingFor<Attempt>(m => m
                    .IndexName("attempts")
                    .IdProperty(x => x.Id))
                .DefaultMappingFor<SantaTracking>(m => m
                    .IndexName("santa-trackings")
                    .IdProperty(x => x.Id))
                .DefaultMappingFor<Zone>(m => m
                    .IndexName("zones")
                    .DisableIdInference())
                .DefaultMappingFor<ObjectInLocationForElastic>(m => m
                    .IndexName("objects")
                    .DisableIdInference())
                .DefaultMappingFor<Participant>(m => m
                    .IndexName("participants")
                    .IdProperty(x => x.EmailAddress));

            if (debugger != null)
            {
                settings = settings
                    .DisableDirectStreaming()
                    .OnRequestCompleted(details =>
                    {
                        if (details.RequestBodyInBytes != null)
                        {
                            debugger("########################## ELASTICSEARCH REQUEST ##########################");

                            debugger(details.HttpMethod + " " + Uri.UnescapeDataString(details.Uri.AbsoluteUri));
                            debugger(Encoding.UTF8.GetString(details.RequestBodyInBytes));
                        }

                        if (details.ResponseBodyInBytes != null)
                        {
                            debugger("########################## ELASTICSEARCH RESPONSE ##########################");

                            debugger(Encoding.UTF8.GetString(details.ResponseBodyInBytes));
                        }
                    });
            }

            return new ElasticClient(settings);
        }
    }
}