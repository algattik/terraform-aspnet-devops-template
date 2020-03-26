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
    public interface ICosmosDBService
    {
        /// <summary>
        ///  Persist a message to Cosmos DB.
        /// </summary>
        /// <returns>Cosmos DB response.</returns>
        public Task<ItemResponse<ComputedSum>> persist(long value, long sum);
    }
}