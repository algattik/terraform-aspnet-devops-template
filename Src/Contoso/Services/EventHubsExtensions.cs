// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

namespace Contoso
{
    using System;
    using Confluent.Kafka;
    using Microsoft.Azure.Management.ResourceManager.Fluent;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Extension methods for Microsoft.Extensions.DependencyInjection.IServiceCollection
    /// that allow adding Application Insights services to application.
    /// </summary>
    public static class EventHubsExtensions
    {
        /// <summary>
        /// Register KafkaProducerService.
        /// </summary>
        /// <param name="services">Service collection to register KafkaProducerService into.</param>
        /// <param name="configuration">Configuration for KafkaProducerService.</param>
        /// <returns>Service collection with KafkaProducerService registered.</returns>
        public static IServiceCollection AddEventHubsProducer(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var clientId = configuration["aadClientId"];
            var clientSecret = configuration["aadClientSecret"];
            var tenantId = configuration["aadTenantId"];
            var producerHubSendAuthorizationRuleResourceId = configuration["providerHubSend"];
            var producerHubTopic = configuration["providerHubTopic"];

            var kafkaProducer = CreateProducer(
                clientId,
                clientSecret,
                tenantId,
                producerHubAuthorizationRuleResourceId,
                producerHubTopic);

            services.AddSingleton(typeof(KafkaProducerService), kafkaProducer);

            return services;
        }

        private static KafkaProducerService CreateProducer(
            string clientId,
            string clientSecret,
            string tenantId,
            string producerHubSendAuthorizationRuleResourceId,
            string producerHubTopic)
        {
            var credentials = SdkContext.AzureCredentialsFactory
                .FromServicePrincipal(
                clientId,
                clientSecret,
                tenantId,
                AzureEnvironment.AzureGlobalCloud);

            var azure = Microsoft.Azure.Management.Fluent.Azure
                .Configure()
                .Authenticate(credentials)
                .WithDefaultSubscription();

            var eventHubNamespaceRule = azure
                .EventHubNamespaces
                .AuthorizationRules
                .GetById(producerHubSendAuthorizationRuleResourceId);

            var ns = eventHubNamespaceRule.NamespaceName;
            var brokerList = $"{ns}.servicebus.windows.net:9093";
            var connStr = eventHubNamespaceRule.GetKeys().PrimaryConnectionString;

            var config = new ProducerConfig
            {
                BootstrapServers = brokerList,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslMechanism = SaslMechanism.Plain,
                SaslUsername = "$ConnectionString",
                SaslPassword = connStr,

                // Debug = "security,broker,protocol"        //Uncomment for librdkafka debugging information
            };
            var producer = new ProducerBuilder<long, string>(config)
                .SetKeySerializer(Serializers.Int64)
                .SetValueSerializer(Serializers.Utf8)
                .Build();
            return new KafkaProducerService(producer, producerHubTopic);
        }
    }
}