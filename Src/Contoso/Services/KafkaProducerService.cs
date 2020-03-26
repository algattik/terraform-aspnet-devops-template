// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

namespace Contoso
{
    using System;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Microsoft.Azure.Management.EventHub.Fluent.Models;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

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
        /// <param name="configuration">Configuration.</param>
        /// <param name="azure">Azure management connection.</param>
        /// <param name="logger">Logger.</param>
        public KafkaProducerService(IConfiguration configuration, AzureService azure, ILogger<KafkaProducerService> logger)
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

            var eventHubNamespaceRule = azure
                .Azure
                .EventHubNamespaces
                .AuthorizationRules
                .GetById(producerHubSendAuthorizationRuleResourceId);

            var ns = eventHubNamespaceRule.NamespaceName;
            var brokerList = $"{ns}.servicebus.windows.net:9093";
            string connStr;
            try
            {
                connStr = eventHubNamespaceRule.GetKeys().PrimaryConnectionString;
            }
            catch (ErrorResponseException e)
            {
                logger.LogCritical(
                    e,
                    "Couldn't retrieve authorization keys for rule {producerHubSendAuthorizationRuleResourceId}",
                    producerHubSendAuthorizationRuleResourceId);
                throw;
            }

            var config = new ProducerConfig
            {
                BootstrapServers = brokerList,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslMechanism = SaslMechanism.Plain,
                SaslUsername = "$ConnectionString",
                SaslPassword = connStr,

                // Debug = "security,broker,protocol"        //Uncomment for librdkafka debugging information
            };
            this.producer = new ProducerBuilder<long, string>(config)
                .SetKeySerializer(Serializers.Int64)
                .SetValueSerializer(Serializers.Utf8)
                .Build();
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