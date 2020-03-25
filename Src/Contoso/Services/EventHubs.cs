
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

namespace Contoso
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Net;
    using Microsoft.Azure.Management.EventHub;
    using Microsoft.Azure.Management.EventHub.Models;
    using Microsoft.Rest.Azure.Authentication;
    using Microsoft.Azure.Management.Fluent;
    using Microsoft.Azure.Management.ResourceManager.Fluent;
    using Microsoft.Extensions.Logging;
    using Confluent.Kafka;
    using Microsoft.ApplicationInsights.AspNetCore.Extensions;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using IP = Confluent.Kafka.IProducer<long, string>;

    //
    // Summary:
    //     Extension methods for Microsoft.Extensions.DependencyInjection.IServiceCollection
    //     that allow adding Application Insights services to application.
    public static class EHExtensions
    {

        public static IServiceCollection AddEventHubsProducer(this IServiceCollection services, IConfiguration configuration)
        {
            var clientId = configuration["aadClientId"];
            var clientSecret = configuration["aadClientSecret"];
            var tenantId = configuration["aadTenantId"];
            var producerHub = configuration["providerHub"];
            var producerHubTopic = configuration["providerHubTopic"];

            var eh = CreateProducer(
                clientId,
                clientSecret,
                tenantId,
                producerHub,
                producerHubTopic);

            services.AddSingleton(typeof(KafkaProducer), eh);

            return services;
        }

        public static KafkaProducer CreateProducer(
            string clientId,
            string clientSecret,
            string tenantId,
            string producerHub,
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
                .GetById(producerHub);

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
                //Debug = "security,broker,protocol"        //Uncomment for librdkafka debugging information
            };
            var producer = new ProducerBuilder<long, string>(config)
                .SetKeySerializer(Serializers.Int64)
                .SetValueSerializer(Serializers.Utf8)
                .Build();
            return new KafkaProducer(producer, producerHubTopic);
        }

    }

    public class KafkaProducer
    {
        private readonly IP producer;

        private readonly string topic;

        public KafkaProducer(IP producer, string topic)
        {
            {
                this.producer = producer;
                this.topic = topic;
            }
        }

        public Task<DeliveryResult<long, string>> ProduceAsync(string msg)
        {
            return producer.ProduceAsync(topic, new Message<long, string> { Key = DateTime.UtcNow.Ticks, Value = msg });
        }
    }
}