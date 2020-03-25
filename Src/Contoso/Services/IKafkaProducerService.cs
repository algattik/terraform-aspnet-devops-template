// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

namespace Contoso
{
    using System.Threading.Tasks;
    using Confluent.Kafka;

    /// <summary>
    /// Service wrapping Confluent.Kafka.IProducer.
    ///
    /// The methods in this class are thread-safe.
    /// </summary>
    public interface IKafkaProducerService
    {
        /// <summary>
        /// Produce a message into Kafka.
        /// </summary>
        /// <param name="msg">Message payload.</param>
        /// <returns>Message delivery result.</returns>
        public Task<DeliveryResult<long, string>> ProduceAsync(string msg);
    }
}