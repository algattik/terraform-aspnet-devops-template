// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

namespace Contoso
{
    using System;
    using System.Threading.Tasks;
    using Confluent.Kafka;

    /// <summary>
    /// Service wrapping Confluent.Kafka.IProducer.
    ///
    /// The methods in this class are thread-safe.
    /// </summary>
    public class KafkaProducerService : IKafkaProducerService
    {
        private readonly IProducer<long, string> producer;

        private readonly string topic;

        /// <summary>
        /// Initializes a new instance of the <see cref="KafkaProducerService"/> class.
        /// </summary>
        /// <param name="producer">Wrapped Confluent.Kafka.IProducer instance.</param>
        /// <param name="topic">Kafka topic to publish messages into.</param>
        public KafkaProducerService(IProducer<long, string> producer, string topic)
        {
            {
                this.producer = producer;
                this.topic = topic;
            }
        }

        /// <summary>
        /// Produce a message into Kafka.
        /// </summary>
        /// <param name="msg">Message payload.</param>
        /// <returns>Message delivery result.</returns>
        public Task<DeliveryResult<long, string>> ProduceAsync(string msg)
        {
            return this.producer.ProduceAsync(
                this.topic,
                new Message<long, string> { Key = DateTime.UtcNow.Ticks, Value = msg });
        }
    }
}