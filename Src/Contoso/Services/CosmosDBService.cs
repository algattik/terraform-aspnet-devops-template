// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

namespace Contoso
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos;
    using Microsoft.Azure.Cosmos.Fluent;
    using Microsoft.Azure.Management.EventHub.Fluent.Models;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Service wrapping Confluent.Kafka.IProducer.
    ///
    /// The methods in this class are thread-safe.
    /// </summary>
    public class CosmosDBService : ICosmosDBService
    {
        private readonly Container container;

        private readonly string topic;

        private readonly ILogger<CosmosDBService> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CosmosDBService"/> class.
        /// </summary>
        /// <param name="configuration">Configuration.</param>
        /// <param name="azure">Azure management connection.</param>
        /// <param name="logger">Logger.</param>
        public CosmosDBService(IConfiguration configuration, AzureService azure, ILogger<CosmosDBService> logger)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (azure == null)
            {
                throw new ArgumentNullException(nameof(azure));
            }

            var producerHubSendAuthorizationRuleResourceId = configuration["providerHubSend"];
            this.topic = configuration["providerHubTopic"];

            logger.LogInformation(
                "Initializing connection to Event Hub {producerHubSendAuthorizationRuleResourceId} topic {topic}",
                producerHubSendAuthorizationRuleResourceId,
                this.topic);

            var cosmosDB = azure
                .Azure
                .CosmosDBAccounts
                .GetById(producerHubSendAuthorizationRuleResourceId)
                ;

            try
            {
                var client = new CosmosClientBuilder(
                    cosmosDB.DocumentEndpoint,
                    cosmosDB.ListKeys().PrimaryMasterKey
                    )
                    .Build();
                this.container = client.GetDatabase("DB").GetContainer("Co");
            }
            catch (ErrorResponseException e)
            {
                logger.LogCritical(
                    e,
                    "Couldn't retrieve authorization keys for rule {producerHubSendAuthorizationRuleResourceId}",
                    producerHubSendAuthorizationRuleResourceId);
                throw;
            }

            this.logger = logger;
        }

        /// <summary>
        ///  Persist a message to Cosmos DB.
        /// </summary>
        /// <returns>Cosmos DB response.</returns>
        public async Task<ItemResponse<ComputedSum>> persist(long value, long sum)
        {
            var e = new ComputedSum
            {
                Id = value.ToString(CultureInfo.InvariantCulture),
                sum = sum
            };
            return await container.ReplaceItemAsync(e, e.Id);
        }
    }
}