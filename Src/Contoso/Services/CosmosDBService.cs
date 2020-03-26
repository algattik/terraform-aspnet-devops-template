// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

namespace Contoso
{
    using System;
    using System.Globalization;
    using System.Linq;
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

            this.logger = logger;

            var cosmosDBContainerResourceId = configuration["cosmosDBContainer"];

            logger.LogInformation(
                "Initializing connection to Cosmos DB container {cosmosDBContainerResourceId}",
                cosmosDBContainerResourceId);

            ParseCosmosDBContainerResourceId(
                cosmosDBContainerResourceId,
                out string cosmosDBResourceId,
                out string cosmosDBDatabase,
                out string cosmosDBContainer);

            var cosmosDB = this.ConnectToCosmosDBAccount(azure, cosmosDBResourceId);
            this.container = this.ConnectToCosmosDBContainer(cosmosDB, cosmosDBDatabase, cosmosDBContainer);
        }

        /// <summary>
        /// Parse a Cosmos DB container ARM Resource ID into its component parts.
        /// </summary>
        /// <param name="cosmosDBContainerResourceId">Cosmos DB container ARM Resource ID.</param>
        /// <param name="cosmosDBResourceId">Cosmos DB account ARM Resource ID.</param>
        /// <param name="cosmosDBDatabase">Cosmos DB database name.</param>
        /// <param name="cosmosDBContainer">Cosmos DB container name.</param>
        public static void ParseCosmosDBContainerResourceId(
                string cosmosDBContainerResourceId,
                out string cosmosDBResourceId,
                out string cosmosDBDatabase,
                out string cosmosDBContainer)
        {
            if (cosmosDBContainerResourceId == null)
            {
                throw new ArgumentNullException(nameof(cosmosDBContainerResourceId));
            }

            var items = cosmosDBContainerResourceId.Split('/');
            cosmosDBResourceId = string.Join('/', items.Take(9));
            cosmosDBDatabase = items[12];
            cosmosDBContainer = items[14];
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
                id = value.ToString(CultureInfo.InvariantCulture),
                Sum = sum,
            };
            return await this.container.UpsertItemAsync(e);
        }

        private ICosmosDBAccount ConnectToCosmosDBAccount(AzureService azure, string cosmosDBResourceId)
        {
            try
            {
                return azure
                    .Azure
                    .CosmosDBAccounts
                    .GetById(cosmosDBResourceId);
            }
            catch (Exception e)
            {
                this.logger.LogCritical(
                    e,
                    "Couldn't retrieve Cosmos DB account keys for rule {cosmosDBResourceId}",
                    cosmosDBResourceId);
                throw;
            }
        }

        private Container ConnectToCosmosDBContainer(ICosmosDBAccount cosmosDB, string cosmosDBDatabase, string cosmosDBContainer)
        {
            try
            {
                var client = new CosmosClientBuilder(
                    cosmosDB.DocumentEndpoint,
                    cosmosDB.ListKeys().PrimaryMasterKey)
                    .Build();
                return client
                    .GetDatabase(cosmosDBDatabase)
                    .GetContainer(cosmosDBContainer);
            }
            catch (Exception e)
            {
                this.logger.LogCritical(
                    e,
                    "Couldn't retrieve Cosmos DB container {cosmosDBContainer}",
                    cosmosDBContainer);
                throw;
            }
        }
    }
}
