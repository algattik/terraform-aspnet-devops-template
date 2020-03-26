// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

namespace Contoso
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Sample service.
    /// </summary>
    public class SampleService : ISampleService
    {
        private readonly ISumComputationAPI client;
        private readonly ILogger logger;
        private readonly MetricsService metrics;
        private readonly ICosmosDBService cosmosDB;

        /// <summary>
        /// Initializes a new instance of the <see cref="SampleService"/> class.
        /// </summary>
        /// <param name="client">Remote controller.</param>
        /// <param name="logger">Logger.</param>
        /// <param name="metrics">Metrics service.</param>
        /// <param name="cosmosDB">Cosmos DB persistence service.</param>
        public SampleService(ISumComputationAPI client, ILogger<SampleService> logger, MetricsService metrics, ICosmosDBService cosmosDB)
        {
            this.client = client;
            this.logger = logger;
            this.metrics = metrics;
            this.cosmosDB = cosmosDB;
        }

        /// <summary>
        /// Add two numbers and return their sum.
        /// </summary>
        /// <param name="value">Number to add values up to.</param>
        /// <returns>Sum of integer numbers from 0 to value.</returns>
        public async Task<int> SumNumbersUpToAsync(int value)
        {
            if (value < 0)
            {
                throw new SumComputationException("Can't sum numbers up to a negative value");
            }

            // Timer to be used to report the duration of a query to.
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var result = await this.SumNumbersUpToInternalAsync(value);
            stopwatch.Stop();
            var duration = stopwatch.Elapsed;

            this.logger.LogInformation("Sum of numbers from 0 to {value} was {result}, computed in {duration}s", value, result, duration);

            await this.cosmosDB.PersistSum(value, result);

            this.metrics?.SumComputationAPICallDuration?.Observe(duration.TotalSeconds);

            return result;
        }

        private async Task<int> SumNumbersUpToInternalAsync(int value)
        {
            if (value <= 1)
            {
                return value;
            }
            else
            {
                var sumUpToValueMinusOne = await this.client.SumNumbersUpTo(value - 1);
                return value + sumUpToValueMinusOne;
            }
        }
    }
}
