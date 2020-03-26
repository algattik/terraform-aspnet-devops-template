// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

namespace Contoso
{
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos;

    /// <summary>
    /// Service wrapping Confluent.Kafka.IProducer.
    ///
    /// The methods in this class are thread-safe.
    /// </summary>
    public interface ICosmosDBService
    {
        /// <summary>
        ///  Persist a message to Cosmos DB.
        /// </summary>
        /// <param name="value">Value up to which the sum is computed.</param>
        /// <param name="sum">Sum of numbers from 0 to value.</param>
        /// <returns>Cosmos DB operation result.</returns>
        public Task<ItemResponse<ComputedSum>> PersistSum(long value, long sum);
    }
}
