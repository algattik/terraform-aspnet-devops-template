// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

namespace Contoso
{
    using System;
    using System.Linq;
    using System.Globalization;
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos;
    using Microsoft.Azure.Cosmos.Fluent;
    using Microsoft.Azure.Management.CosmosDB.Fluent;
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

            var cosmosDBContainerResourceId = configuration["cosmosDBContainer"];

            logger.LogInformation(
                "Initializing connection to Event Hub {cosmosDBContainerResourceId} topic {topic}",
                cosmosDBContainerResourceId,
                this.topic);

            ParseCosmosDBContainerResourceId(
                cosmosDBContainerResourceId,
                out string cosmosDBResourceId,
                out string cosmosDBDatabase,
                out string cosmosDBContainer);

            ICosmosDBAccount cosmosDB;
            try
            {
                cosmosDB = azure
                    .Azure
                    .CosmosDBAccounts
                    .GetById(cosmosDBResourceId);
            }
            catch (Exception e)
            {
                logger.LogCritical(
                    e,
                    "Couldn't retrieve Cosmos DB account keys for rule {cosmosDBResourceId}",
                    cosmosDBResourceId);
                throw;
            }

            try
            {
                var client = new CosmosClientBuilder(
                    cosmosDB.DocumentEndpoint,
                    cosmosDB.ListKeys().PrimaryMasterKey)
                    .Build();
                this.container = client
                    .GetDatabase(cosmosDBDatabase)
                    .GetContainer(cosmosDBContainer);
            }
            catch (Exception e)
            {
                logger.LogCritical(
                    e,
                    "Couldn't retrieve authorization keys for rule {cosmosDBContainerResourceId}",
                    cosmosDBContainerResourceId);
                throw;
            }

            this.logger = logger;
        }

        /// <summary>
        ///  Persist a message to Cosmos DB.
        /// </summary>
        /// <param name="value">Value up to which the sum is computed.</param>
        /// <param name="sum">Sum of numbers from 0 to value.</param>
        /// <returns>Cosmos DB operation result.</returns>
        public async Task<ItemResponse<ComputedSum>> PersistSum(long value, long sum)
        {
            var e = new ComputedSum
            {
                Id = value.ToString(CultureInfo.InvariantCulture),
                Sum = sum,
            };
            return await this.container.ReplaceItemAsync(e, e.Id);
        }

        public static void ParseCosmosDBContainerResourceId(
                string cosmosDBContainerResourceId,
                        out string cosmosDBResourceId,
                        out string cosmosDBDatabase,
                        out string cosmosDBContainer)
        {
            var items = cosmosDBContainerResourceId.Split('/');
            cosmosDBResourceId = string.Join('/', items.Take(4));
            cosmosDBDatabase = items[5];
            cosmosDBContainer = items[6];
        }
    }
}
