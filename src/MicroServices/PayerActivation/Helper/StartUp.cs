using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Prem.N3.PayerActivation.Helper;
using Prem.N3.PayerActivation.Processors;
using System;

[assembly: WebJobsStartup(typeof(StartUp))]
namespace Prem.N3.PayerActivation.Helper
{
    public class StartUp : IWebJobsStartup
    {
        private const string CosmosDBEndpoint = "CosmosDBEndpoint";
        private const string CosmosDBAuthKey = "CosmosDBAuthKey";

        public void Configure(IWebJobsBuilder builder)
        {
            builder.Services.AddTransient(typeof(IProcessor), typeof(Processor));
            builder.Services.AddTransient(typeof(ICosmosDbRepository<>), typeof(CosmosDbRepository<>));
            builder.Services.AddTransient(typeof(ITopicClientFactory), typeof(TopicClientFactory));
            builder.Services.AddTransient(typeof(IErrorTopicPublisher), typeof(ErrorTopicPublisher));
            RegisterCosmosClient(builder);
        }

        private static void RegisterCosmosClient(IWebJobsBuilder builder)
        {
            string cosmosDbEndpoint = Environment.GetEnvironmentVariable(CosmosDBEndpoint);
            string cosmosDbAuthKey = Environment.GetEnvironmentVariable(CosmosDBAuthKey);

            builder.Services.AddSingleton(s => new CosmosClient(cosmosDbEndpoint, cosmosDbAuthKey,
                new CosmosClientOptions { AllowBulkExecution = true }));
        }
    }
}
