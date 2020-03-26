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

            var containerId = configuration["cosmosDBContainer"];

            logger.LogWarning(
                "Skipping connection to Cosmos DB container {containerId} (not yet implemented)",
                containerId);

            this.logger = logger;
        }

        /// <summary>
        ///  Persist a message to Cosmos DB.
        /// </summary>
        /// <param name="value">Value up to which the sum is computed.</param>
        /// <param name="sum">Sum of numbers from 0 to value.</param>
        /// <returns>Cosmos DB operation result.</returns>
        public Task PersistSum(long value, long sum)
        {
            this.logger.LogWarning(
                "Saving to Cosmos DB not yet implemented");
            return Task.FromResult(0);
        }
    }
}
