using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;
using XMAS2019.Api;
using XMAS2019.Domain.Infrastructure;
using XMAS2019.Domain.Services;

[assembly: WebJobsStartup(typeof(Startup))]
namespace XMAS2019.Api
{
    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            string environment = Environment.GetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT");

            builder.Services.AddSingleton(_ => ElasticClientFactory.Create());
            builder.Services.AddSingleton(_ => DocumentClientFactory.Create());
            builder.Services.AddSingleton<IAttemptService, AttemptService>();
            builder.Services.AddSingleton<IZoneService, ZoneService>();
            builder.Services.AddSingleton<IToyDistributionProblemRepository, ToyDistributionProblemRepository>();

            if (!string.Equals(environment, "Development", StringComparison.OrdinalIgnoreCase))
            {
                builder.Services.AddLogging(loggingBuilder => loggingBuilder
                    .AddSerilog(new LoggerConfiguration()
                        .WriteTo
                            .Elasticsearch(new ElasticsearchSinkOptions(ElasticClientFactory.Url)
                            {
                                AutoRegisterTemplate = true,
                                AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                                MinimumLogEventLevel = LogEventLevel.Information,
                                ModifyConnectionSettings = connection =>
                                {
                                    connection.BasicAuthentication(ElasticClientFactory.Username, ElasticClientFactory.Password);

                                    return connection;
                                }
                            })
                        .CreateLogger()));
            }
        }
    }
}